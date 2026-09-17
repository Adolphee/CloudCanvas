from dataclasses import asdict
import json, logging
from typing import Any
from azure.servicebus import ServiceBusMessage
from azure.servicebus.aio import ServiceBusClient
from application.exceptions import MessageSendFailureException
from application.ports import Messenger
from domain.models import CCEventMessage, Photo
from infrastructure.mapper import Mapper

class ServiceBusService(Messenger):
    def __init__(self, client: ServiceBusClient):
        self.client = client

    async def send_message(self, queue_name: str, message: CCEventMessage):
        msg = Mapper.to_infra_model(message)
        async with self.client.get_queue_sender(queue_name) as sender:
            try: await sender.send_messages(msg)
            except MessageSendFailureException as e:
                e.message="Failed to send message."
                logging.exception(e.message)
                raise

    async def notify_enrichment_complete(self, photo: Photo):
        props: dict[str | bytes, Any] = {
                "enrichment_status": "complete",
                "results": json.dumps({
                    "caption": photo.caption,
                    "tags": [tag.name for tag in photo.tags]
                })
            }
        
        msg = ServiceBusMessage(
            id=photo.id,
            subject="intelligence",
            content_type="application/json",
            correlation_id=photo.user_id,
            session_id=photo.id,
            application_properties=props,
            body=json.dumps(asdict(photo), indent=2)
        )
        try:
            async with self.client.get_queue_sender("file-updates") as sender:
                await sender.send_messages(msg)
        except MessageSendFailureException as e:
            e.message="Failed to notify enrichment complete."
            logging.exception(e.message)
            raise

    async def close_connection(self): await self.client.close()