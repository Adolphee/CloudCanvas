import os
import logging as logger
import azure.functions as func
from azure.identity.aio import DefaultAzureCredential
from application.ports.closable import Closable
from application.service_ops import init_services, close_services
from application.validation import Validator
from application.exceptions import AppSettingsNotFoundException
from domain.constants import Constants
from domain.models import Photo
from application.generate_tags import generate_tags

# use case orchestration
# Generate image enrichment (refinement WIP)
# 1. Validate the incoming event. --> ✅
# 2. Ask ImageAnalyzer for tags and caption. ✅
# 3. Apply confidence and moderation policy. --> TODO! (good flex)
# 4. Persist enriched state. --> done: projection, TODO: persistence
# 5. Publish ImageEnriched (event, messaging). --> ✅
# 6. Make processing idempotent. --> ✅
#   --> means: processing the same event multiple times should have the same result
#   --> TODO: implement in the function itself (ServiceBus message lock token)
###
A = Constants.Attr
P = Constants.AppSettings
TOPIC = os.environ.get(P.SB_TOPIC)
SUB = os.environ.get(P.SBSUB_TAG)

blueprint = func.Blueprint()


@blueprint.function_name(name="generate_ai_tags")
@blueprint.service_bus_topic_trigger("message", P.SB_CONN, str(TOPIC), str(SUB), is_sessions_enabled=True)
async def handle_tagging_enrichment(message: func.ServiceBusMessage):
    if not TOPIC or not SUB:
        raise AppSettingsNotFoundException(
            setting_name="ServiceBus Topic & Subscription")
    payload_photo: Photo = Validator.validate_enrichment_request(
        message.get_body())  # Fail fast principle
    credential = DefaultAzureCredential()
    S = Constants.Services
    services: dict[str, Closable] = {}
    try:
        analyzer, persistence, projector, messenger = init_services(credential)
        services = {S.ANALYZER: analyzer, S.PROJECTOR: projector, S.MESSENGER: messenger}
        proj_check = await projector.verify_no_prior_enrichment(payload_photo.id, payload_photo.user_id)
        pers_check = persistence.verify_no_prior_enrichment(payload_photo.id)
        logger.critical("Completed: %a and %a, Photo: %a", pers_check, proj_check.is_completed, proj_check.photo)
        if pers_check and proj_check.is_completed:
            logger.info("Enrichment already completed for %a. Skipping...", payload_photo.id)
            return
        photo = proj_check.photo or payload_photo
        logger.info(f"Generating AI tags for image: {photo.url}")
        tags = await generate_tags(analyzer, photo.url)
        for tag in tags:
            logger.info(f"Generated Tag: {tag.name} with confidence: {tag.confidence}")
            if (tag.name not in [t.name for t in photo.tags]): photo.tags.append(tag)
        logger.info(f"Saving {len(photo.tags)} tags to persistence store...")
        persistence.update_smartTags(photo.id, [t.name for t in photo.tags])
        logger.info(f"Saving {len(photo.tags)} tags to projection store...")
        await projector.project_tags(photo)
        logger.info(f"Tags projected. Sending notification...")
        await messenger.notify_enrichment_complete(photo)
    finally:  # "thank you for your service"
        logger.debug("Cleaning up...")
        await close_services(services, credential)
        logger.info("Done.")
