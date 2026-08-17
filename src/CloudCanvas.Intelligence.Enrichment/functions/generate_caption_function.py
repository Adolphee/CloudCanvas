import os
import azure.functions as func
from application.generate_caption import generate_caption
from infrastructure.composition_root import build_image_analyzer

SB_CONN = "SBConnection"
TOPIC = str(os.environ.get("SBTopicName"))
SUB = str(os.environ.get("SBSubscription_caption"))
MAX_SIZE = int(os.environ.get("MAX_MESSAGE_SIZE", 1024 * 1024))  # Default to 1 MB if not set

blueprint = func.Blueprint()
analyzer = build_image_analyzer()

@blueprint.function_name("generate_ai_caption")
@blueprint.service_bus_topic_trigger(
    arg_name="message",
    connection=SB_CONN,
    topic_name=TOPIC,
    subscription_name=SUB,
    is_sessions_enabled=True, 
    max_message_size=MAX_SIZE
)
async def handle_caption_enrichment(message: func.ServiceBusMessage):
    image_url = message.get_body().decode()
    await generate_caption(analyzer, image_url)