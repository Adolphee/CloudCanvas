from azure.identity.aio import DefaultAzureCredential
from application.ports import ImageAnalyzer, PersistenceService, Messenger, ProjectionService, Closable
from infrastructure.composition_root import build_image_analyzer, build_messenger, build_projection_service, build_persistence_store
import logging as logger


def init_services(credential: DefaultAzureCredential) -> tuple[ImageAnalyzer, PersistenceService, ProjectionService, Messenger]:
    analyzer = build_image_analyzer(credential)
    projector = build_projection_service(credential)
    messenger = build_messenger(credential)
    persistence = build_persistence_store()
    return analyzer, persistence, projector, messenger

async def close_services(services: dict[str, Closable], credential: DefaultAzureCredential):
    for name, service in services.items(): 
        try: await service.close_connection()
        except Exception as e: 
            logger.exception("Failed to close the %a connection: %a", name, e)
    try: await credential.close()
    except Exception as e: 
        logger.exception("Failed to close the Credential: %a", e)