import logging as logger
from application.exceptions import *
from azure.cosmos.partition_key import PartitionKeyType
from domain.models import Photo, PhotoVerificationResult as Verification
from domain.constats import Constants
from azure.cosmos.aio import CosmosClient, ContainerProxy
from application.ports.projection_service import ProjectionService
from azure.cosmos.exceptions import CosmosHttpResponseError

DB_NAME = "cloudcanvas"
CONTAINER= "user_photos"
class CosmosService(ProjectionService):
    def __init__(self, client: CosmosClient): self.client = client
    
    async def project_tags(self, image: Photo) -> bool:
        container = self.__get_container()
        operation = {
                "path" : "/smartTags",
                "op" : "add",
                "value" : [tag.name for tag in image.tags]
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
                "path" : "/smartCaption",
                "op" : "set",
                "value" : image.caption
            }
        try:
            res = await container.patch_item(image.id, image.user_id, [operation])
            logger.info("Projected caption successfully.")
            return len(res) > 0
        except Exception:
            logger.exception("Caption projection failed.")
            raise

    def __get_container(self, name: str = CONTAINER, db: str = DB_NAME) -> ContainerProxy:
        db_client = self.client.get_database_client(db)     # I could use create-if-not-exists here but...
        container = db_client.get_container_client(name)    # ... these functions shouldn't in the absence of these resources
        if not db_client or not container:  
            logger.exception(f"Unable to retrieve container {name} from {db}")
            raise 
        return container

    async def close_connection(self):
        logger.debug("Closing CosmosService connection...")
        await self.client.close()

    # TODO: check in cosmos_db if this photo has already been enriched
    # The most cost-efficient way is to just get the two attributes I know are AI generated on the object
    # ⚠️ Might be risky but a goal here is to weigh the pros and cons of adding an optional parameter 'properties'
    # ... which allows the caller to specify which properties should be checked
    # --> Might also come in handy later if I'm able to pull it off
    async def verify_no_prior_enrichment(self, photo_id: str, user_id: PartitionKeyType) -> Verification:
        container = self.__get_container()
        operation = "Verify_no_prior_enrichment"
        try:  
            item = await container.read_item(photo_id, partition_key=user_id)
            if item:
                A = Constants.Attr
                photo = Photo(
                    id=item[A.ID],
                    user_id=item[A.USER_ID],
                    tags=item[A.TAGS],
                    url=item[A.URL],
                    caption=item[A.CAPTION]
                )
                is_completed = not (not photo.caption or not photo.tags)
                return Verification(is_completed, photo)
            else: 
                msg = f"Photo not found in projection-store:{photo_id}"
                logger.critical(msg)
                raise EnrichmentException(msg=msg, operation=operation)
        except Exception | CosmosHttpResponseError as e:
            logger.exception("Exception while verifying prior enrichment item %a from container %a", photo_id, container.id)
            raise EnrichmentException(msg=f"Photo not found: {photo_id}", operation=operation) from e
        