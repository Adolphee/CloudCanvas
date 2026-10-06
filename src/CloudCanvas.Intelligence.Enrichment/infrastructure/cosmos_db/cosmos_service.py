import os, logging as logger
from application.exceptions import *
from azure.cosmos.partition_key import PartitionKeyType
from domain.models import Photo, PhotoVerificationResult as Verification
from domain.constants import Constants
from azure.cosmos.aio import CosmosClient, ContainerProxy
from application.ports.projection_service import ProjectionService
from azure.cosmos.exceptions import CosmosHttpResponseError
from application.mapper import Mapper
DB_NAME = "cloudcanvas"
CONTAINER = "user_photos"


class CosmosService(ProjectionService):
    def __init__(self, client: CosmosClient): self.client = client

    async def project_tags(self, image: Photo) -> bool:
        container = self.__get_container()
        logger.info("Projecting tags for photo: %a", image.id)
        operation = {
            "path": "/smartTags",
            "op": "add",
            "value": [tag.toJSON() for tag in image.tags]
        }
        try:
            res = await container.patch_item(image.id, image.user_id, [operation])
            logger.info("Projected tags successfully.")
            return len(res) > 0
        except Exception as e:
            logger.exception("Tags projection failed.")
            raise

    async def project_caption(self, image: Photo) -> bool:
        container = self.__get_container()
        operation = {
            "path": "/smartCaption",
            "op": "set",
            "value": image.caption
        }
        try:
            res = await container.patch_item(image.id, image.user_id, [operation])
            logger.info("Projected caption successfully.")
            return len(res) > 0
        except Exception:
            logger.exception("Caption projection failed.")
            raise

    def __get_container(self, name: str = CONTAINER, db: str = DB_NAME) -> ContainerProxy:
        S = Constants.AppSettings
        db = os.getenv(S.PROJECTION_DB_NAME) or db
        name = os.getenv(S.PROJ_PHOTOS_CONTAINER) or name

        try:
            # I could use create-if-not-exists here but...
            db_client = self.client.get_database_client(db)
            # ... these functions shouldn't be triggered in the absence of these resources
            container = db_client.get_container_client(name)
            return container
        except CosmosHttpResponseError as e:
            logger.critical("Unable to retrieve container %a from %a: %a", name, db, e.message)
            raise

    async def close_connection(self):
        logger.debug("Closing CosmosService connection...")
        await self.client.close()

    async def verify_no_prior_enrichment(self, photo_id: str, user_id: PartitionKeyType) -> Verification:
        container = self.__get_container()
        operation = "Verify_no_prior_enrichment"
        try:
            res_array = await container.read_item(photo_id, partition_key=user_id)
            if res_array:
                A = Constants.Attr
                photo = Mapper.to_photo(res_array)
                is_completed = bool(photo and len(photo.tags) > 0 and photo.caption)
                return Verification(is_completed, photo)
            else:
                msg = "Photo not found in projection-store: %a"
                logger.exception(msg, photo_id)
                raise EnrichmentException(msg=msg, operation=operation)
        except Exception as e:
            logger.exception("Exception while verifying prior enrichment item %a from container %a/%a", photo_id, DB_NAME, container.id)
            raise EnrichmentException(msg=f"Photo not found: {photo_id}", operation=operation) from e
