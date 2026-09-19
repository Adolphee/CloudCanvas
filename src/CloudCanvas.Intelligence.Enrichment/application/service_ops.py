from azure.identity.aio import DefaultAzureCredential
from application.ports.closable import Closable
from application.ports.image_analyzer import ImageAnalyzer
from application.ports.messenger import Messenger
from application.ports.projection_service import ProjectionService
from domain.constats import Constants
from infrastructure.composition_root import build_image_analyzer, build_messenger, build_projection_service
import logging as logger

async def init_services(credential: DefaultAzureCredential) -> tuple[ImageAnalyzer, ProjectionService, Messenger]:
    analyzer = await build_image_analyzer(credential)
    projector = await build_projection_service(credential)
    messenger = await build_messenger(credential)
    return analyzer, projector, messenger

async def close_services(services: dict[str, Closable], credential: DefaultAzureCredential):
    for name, service in services.items(): 
        try: await service.close_connection()
        except Exception as e: 
            logger.exception("Failed to close the %a connection", name)
    try: await credential.close()
    except Exception as e: 
        logger.exception("Failed to close the Credential")