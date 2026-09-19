import os, logging as logger
import azure.functions as func
from azure.identity.aio import DefaultAzureCredential
from application.ports.closable import Closable
from application.service_ops import init_services, close_services
from application.validation import Validator
from application.exceptions import AppSettingsNotFoundException
from domain.constats import Constants
from domain.models import Photo
from application.generate_tags import generate_tags

### use case orchestration    
## Generate image enrichment (refinement WIP)
# 1. Validate the incoming event. --> ✅ 
# 2. Ask ImageAnalyzer for tags and caption. ✅
# 3. Apply confidence and moderation policy. --> TODO! (good flex)
# 4. Persist enriched state. --> done: projection, TODO: persistence
# 5. Publish ImageEnriched (event, messaging). --> ✅
# 6. Make processing idempotent. --> ✅
#   --> means: processing the same event multiple times should have the same result
#   --> TODO: implement in the function itself (ServiceBus message lock token)
###
A = Constants.AppSettings
TOPIC = os.environ.get(A.SB_TOPIC)
SUB = os.environ.get(A.SBSUB_TAG)

blueprint = func.Blueprint()
@blueprint.function_name(name="generate_ai_tags")
@blueprint.service_bus_topic_trigger("message", A.SB_CONN, str(TOPIC), str(SUB), is_sessions_enabled=True)
async def handle_tagging_enrichment(message: func.ServiceBusMessage):
    if not TOPIC or not SUB: raise AppSettingsNotFoundException(setting_name="ServiceBus Topic & Subscription")
    photo: Photo = Validator.validate_enrichment_request(message.get_body()) #Fail fast principle
    credential = DefaultAzureCredential()
    S = Constants.Services
    services: dict[str, Closable] = {}
    try:
        analyzer, projector, messenger = await init_services(credential)
        services = { S.ANALYZER: analyzer, S.PROJECTOR: projector, S.MESSENGER: messenger }
        verification = await projector.verify_no_prior_enrichment(photo.id, photo.user_id)
        if verification.is_completed: 
            logger.critical("Enrichment already completed for %a", photo.id)
            return
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
    finally: ## "thank you for your service"
        logger.debug("Cleaning up...")
        await close_services(services, credential)
        logger.info("Done.")