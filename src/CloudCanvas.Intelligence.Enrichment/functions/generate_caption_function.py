import os, logging
import azure.functions as func
from domain.models import Photo
from application.exceptions import InvalidPayloadException, BadRequestException
from application.validation import Validator
from application.generate_caption import generate_caption
from infrastructure.composition_root import build_image_analyzer, build_projection_service

SB_CONN = "SB_CONN"
TOPIC = str(os.environ.get("SB_TOPIC"))
SUB = str(os.environ.get("SBSUB_CAP"))

async def init_services():
    global analyzer, projector, validator
    analyzer = analyzer or await build_image_analyzer()
    projector = projector or  await build_projection_service()
    validator = validator or Validator()

blueprint = func.Blueprint()
@blueprint.function_name("generate_ai_caption")
@blueprint.service_bus_topic_trigger("message", SB_CONN, TOPIC, SUB, is_sessions_enabled=True)
@blueprint.retry(strategy="fixed_delay", max_retry_count="0", delay_interval="00:00:01") #debugging only, remove for production
async def handle_caption_enrichment(message: func.ServiceBusMessage):
    await init_services()
    photo: Photo = validator.validate_enrichment_request(message.get_body())
    logging.info(f"Generating AI caption for image: {photo.url}")
    photo.caption = await generate_caption(analyzer, photo.url)
    logging.info(f"Saving caption to projection...")
    res = await projector.project_caption(photo) #Next: Notify service bus
    logging.info("Done.")
    