import os, logging as logger
import azure.functions as func
from application.service_ops import init_services, close_services
from application.validation import Validator
from domain.models import Photo
from application.generate_tags import generate_tags

SB_CONN = "SB_CONN"
TOPIC = str(os.environ.get("SB_TOPIC"))
SUB = str(os.environ.get("SBSUB_TAG"))

### use case orchestration    
## Generate image enrichment (refinement WIP)
# 1. Validate the incoming event. --> ✅ 
# 2. Ask ImageAnalyzer for tags and caption. ✅
# 3. Apply confidence and moderation policy. --> TODO! (good flex)
# 4. Persist enriched state. --> done: projection, TODO: persistence
# 5. Publish ImageEnriched (event, messaging). --> ✅
# 6. Make processing idempotent. 
#   --> means: processing the same event multiple times should have the same result
#   --> TODO: implement in the function itself (ServiceBus message lock token)
###


blueprint = func.Blueprint()
@blueprint.function_name(name="generate_ai_tags")
@blueprint.service_bus_topic_trigger("message", SB_CONN, TOPIC, SUB, is_sessions_enabled=True)
async def handle_tagging_enrichment(message: func.ServiceBusMessage):
    photo: Photo = Validator.validate_enrichment_request(message.get_body()) #Fail fast principle
    analyzer, projector, messenger = await init_services()
    verification = await projector.verify_no_prior_enrichment(photo.id, photo.user_id)
    if verification.is_complered: logger.critical("Enrichment already completed for %a", photo.id)
    else:
        photo = verification.photo or photo
        logger.info(f"Generating AI tags for image: {photo.url}")
        tags = await generate_tags(analyzer, photo.url)
        for tag in tags:
            logger.info(f"Generated Tag: {tag.name}")
            if(tag.name not in [t.name for t in photo.tags]): photo.tags.append(tag)
        await projector.project_tags(photo)
        logger.info(f"Tags projected. Sending notification...")
        await messenger.notify_enrichment_complete(photo)
        logger.info("Notification sent. Cleaning up...")
    await close_services([analyzer, projector, messenger])
    logger.info("Done.")