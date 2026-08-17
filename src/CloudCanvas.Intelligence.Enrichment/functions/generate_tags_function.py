import os
import logging
import azure.functions as func
from domain.models import ImageTag
from application.generate_tags import generate_tags
from infrastructure.composition_root import build_image_analyzer

TOPIC_NAME = str(os.environ.get("SBTopicName"))
SUBSCRIPTION_NAME = str(os.environ.get("SBSubscription_tag"))
SBCONNECTION_STRING = "SBConnection"
MAX_SIZE = int(os.environ.get("MAX_MESSAGE_SIZE", 1024 * 1024))  # Default to 1 MB if not set

blueprint = func.Blueprint()
analyzer = build_image_analyzer()

@blueprint.function_name(name="generate_ai_tags")
@blueprint.service_bus_topic_trigger(
    arg_name="message",
    topic_name=TOPIC_NAME,
    subscription_name=SUBSCRIPTION_NAME,
    connection=SBCONNECTION_STRING,
    is_sessions_enabled=True, 
    max_message_size=MAX_SIZE
)
async def handle_tagging_enrichment(message: func.ServiceBusMessage):
    image_url = message.get_body().decode()
    logging.info(f"Processing image URL: {image_url}")
    tags = await generate_tags(analyzer, image_url)
    for tag in tags:
        logging.info(f"Generated Tag: {tag.name}")