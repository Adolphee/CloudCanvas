from application.ports.closable import Closable
from application.ports.image_analyzer import ImageAnalyzer
from application.ports.messenger import Messenger
from application.ports.projection_service import ProjectionService
from application.validation import Validator
from infrastructure.composition_root import build_image_analyzer, build_messenger, build_projection_service


async def init_services() -> tuple[ImageAnalyzer, ProjectionService, Messenger]:
    services = {
        "analyzer": await build_image_analyzer(),
        "projector": await build_projection_service(),
        "messenger": await build_messenger()
    }
    return services["analyzer"], services["projector"], services["messenger"]

async def close_services(services: list[Closable]):
    for service in services: await service.close_connection()