import os
import logging
import azure.functions as func

TOPIC_NAME = os.environ["SBTopicName"]
SUBSCRIPTION_NAME = os.environ["SBSubscription"]
SBCONNECTION_STRING = "SBConnection"

app = func.FunctionApp()

@app.service_bus_topic_trigger(
    arg_name="msg",
    topic_name=TOPIC_NAME,
    subscription_name=SUBSCRIPTION_NAME,
    connection=SBCONNECTION_STRING,
    is_sessions_enabled=True)
def handle_topic_message(msg: func.ServiceBusMessage):
    # Process the message
    logging.info(f"Received message: {msg.get_body().decode('utf-8')}")