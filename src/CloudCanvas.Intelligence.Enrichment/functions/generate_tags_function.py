import os, json, logging
import azure.functions as func
from domain.models import Photo
from application.generate_tags import generate_tags
from infrastructure.composition_root import build_image_analyzer, build_projection_service

SB_CONN = "SB_CONN"
TOPIC = str(os.environ.get("SB_TOPIC"))
SUB = str(os.environ.get("SBSUB_TAG"))

blueprint = func.Blueprint()
analyzer = build_image_analyzer()
projector = build_projection_service()

@blueprint.function_name(name="generate_ai_tags")
@blueprint.service_bus_topic_trigger("message", SB_CONN, TOPIC, SUB, is_sessions_enabled=True)
@blueprint.retry(strategy="fixed_delay", max_retry_count="0", delay_interval="00:00:01")
async def handle_tagging_enrichment(message: func.ServiceBusMessage):
    body = json.loads(message.get_body().decode())
    image = Photo(**body) #todo: more validation needed for id & user_id
    logging.info(f"Generating AI tags for image: {image.url}")
    tags = await generate_tags(analyzer, image.url)
    for tag in tags:
        logging.info(f"Generated Tag: {tag.name}")
        if(tag.name not in [t.name for t in image.tags]): image.tags.append(tag)
        
    logging.info(f"Saving tags to projection...")
    await projector.project_tags(image)
    logging.info("Done.") 