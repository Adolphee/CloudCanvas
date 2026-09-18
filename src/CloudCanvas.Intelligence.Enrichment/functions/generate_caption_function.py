import os, logging as logger
import azure.functions as func
from application.validation import Validator
from domain.models import Photo, PhotoVerificationResult as Verification
from application.generate_caption import generate_caption
from application.service_ops import init_services, close_services
from domain.constats import Constants
SB_CONN = "SB_CONN"
TOPIC = str(os.environ.get("SB_TOPIC"))
SUB = str(os.environ.get("SBSUB_CAP"))

blueprint = func.Blueprint()
@blueprint.function_name("generate_ai_caption")
@blueprint.service_bus_topic_trigger("message", SB_CONN, TOPIC, SUB, is_sessions_enabled=True)
async def handle_caption_enrichment(message: func.ServiceBusMessage):
    photo: Photo = Validator.validate_enrichment_request(message.get_body()) #Fail fast principle
    analyzer, projector, messenger = await init_services()
    verification = await projector.verify_no_prior_enrichment(photo.id, photo.user_id)
    if verification.is_complered: logger.critical("Enrichment already completed for %a", photo.id)
    else:
        photo = verification.photo or photo
        logger.info(f"Generating AI caption for image: {photo.url}")
        photo.caption = await generate_caption(analyzer, photo.url)
        logger.info(f"Saving caption to projection...")
        res = await projector.project_caption(photo)
        logger.info(f"Caption projected. Sending notification...")
        await messenger.notify_enrichment_complete(photo)
        logger.info("Notification sent. Cleaning up...") 
    await close_services([analyzer, projector, messenger])
    logger.info("Done.")