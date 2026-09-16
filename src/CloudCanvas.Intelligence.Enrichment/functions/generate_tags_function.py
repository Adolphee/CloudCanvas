import os, json, logging
import azure.functions as func
from application.validation import Validator
from domain.models import Photo
from application.generate_tags import generate_tags
from infrastructure.composition_root import build_image_analyzer, build_projection_service

SB_CONN = "SB_CONN"
TOPIC = str(os.environ.get("SB_TOPIC"))
SUB = str(os.environ.get("SBSUB_TAG"))

async def init_services():
    global analyzer, projector, validator
    analyzer = analyzer or await build_image_analyzer()
    projector = projector or await build_projection_service()
    validator = validator or Validator()

### use case orchestration    
## Generate image enrichment (refinement WIP)
# 1. Validate the incoming event. --> TODO 
# 2. Ask ImageAnalyzer for tags and caption.
# 3. Apply confidence and moderation policy. --> TODO! (good flex)
# 4. Persist enriched state. --> done: projection, TODO: persistence
# 5. Publish ImageEnriched (event, messaging). --> TODO
# 6. Make processing idempotent. 
#   --> means: processing the same event multiple times should have the same result
#   --> TODO: implement in the function itself (ServiceBus message lock token)
###


blueprint = func.Blueprint()
@blueprint.function_name(name="generate_ai_tags")
@blueprint.service_bus_topic_trigger("message", SB_CONN, TOPIC, SUB, is_sessions_enabled=True)
@blueprint.retry(strategy="fixed_delay", max_retry_count="0", delay_interval="00:00:01")
async def handle_tagging_enrichment(message: func.ServiceBusMessage):
    await init_services()
    photo: Photo = validator.validate_enrichment_request(message.get_body())
    tags = await generate_tags(analyzer, photo.url)
    for tag in tags:
        logging.info(f"Generated Tag: {tag.name}")
        if(tag.name not in [t.name for t in photo.tags]): photo.tags.append(tag)
    logging.info(f"Saving tags to projection...")
    await projector.project_tags(photo)
    logging.info("Done.")