import logging
from domain.models import Photo
from azure.cosmos.aio import CosmosClient, ContainerProxy
from application.ports.projection_service import ProjectionService

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
            logging.info("Projected tags successfully.")
            return len(res) > 0
        except Exception as e:
            logging.exception("Tags projection failed.")
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
            logging.info("Projected caption successfully.")
            return len(res) > 0
        except Exception:
            logging.exception("Caption projection failed.")
            raise

    def __get_container(self, name: str = "user_photos", db: str = "cloudcosmos_sql") -> ContainerProxy:
        db_client = self.client.get_database_client(db)
        if db_client is None: 
            logging.exception(f"CosmosDB unable to retrieve database {db}")
            raise
        container = db_client.get_container_client(name)
        if container is None:  
            logging.exception(f"CosmosDB unable to retrieve container {name} from {db}")
            raise 
        return container