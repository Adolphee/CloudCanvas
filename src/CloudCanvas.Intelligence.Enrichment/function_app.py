import os
import logging
import azure.functions as func
from infrastructure.composition_root import build_image_analyzer

TOPIC_NAME = str(os.environ.get("SBTopicName"))
SUBSCRIPTION_NAME = str(os.environ.get("SBSubscription"))
SBCONNECTION_STRING = "SBConnection"
max_size = int(os.environ.get("MAX_MESSAGE_SIZE", 1024 * 1024))  # Default to 1 MB if not set

app = func.FunctionApp()
image_analyzer = build_image_analyzer()

@app.function_name(name="generate_ai_tags")
@app.service_bus_topic_trigger(
    aync=True,
    arg_name="message",
    topic_name=TOPIC_NAME,
    subscription_name=SUBSCRIPTION_NAME,
    connection=SBCONNECTION_STRING,
    is_sessions_enabled=True, 
    max_message_size=max_size
)
async def handle_topic_message(message: func.ServiceBusMessage):
    image_url = message.get_body().decode('utf-8')
    logging.info(f"Processing image URL: {image_url}")
    tags = await image_analyzer.generate_tags(image_url)
    for tag in tags:
        logging.info(f"Generated Tag: {tag}")
