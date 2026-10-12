import json
import logging
from typing import Any
from uuid import UUID
from domain.models import CCEventMessage
from azure.servicebus import ServiceBusMessage

class Mapper:
    def __init__(self):
        self.logger = logging.getLogger(__name__)

    @staticmethod
    def map_to_domain_model(message: ServiceBusMessage) -> CCEventMessage:
        props = dict[str | bytes, Any]()
        message_props = message.application_properties
        if message_props:
            for name, val in message_props.items(): props[str(name)] = val

        return CCEventMessage(
            id=message.message_id or str(),
            subject=message.subject or  str(),
            content_type=message.content_type or str(),
            correlation_id=message.correlation_id or str(),
            session_id=message.session_id or str(),
            properties=props,
            body=message.body
        )

    @staticmethod
    def to_infra_model(eventMessage: CCEventMessage) -> ServiceBusMessage:
        message = ServiceBusMessage(json.dumps(eventMessage.body))
        message.message_id = eventMessage.id or str(UUID())
        message.session_id = eventMessage.session_id or str(UUID())
        message.correlation_id = eventMessage.correlation_id or str(UUID())
        message.application_properties = Mapper._convert_msg_properties(eventMessage.properties)
        return message

    @staticmethod
    def _convert_msg_properties(props: dict[str | bytes, Any]) -> dict[str | bytes, Any]:
        converted_props = dict[str | bytes, Any]()
        for name, val in props.items():
            converted_props[str(name)] = val
        return converted_props

    
    @staticmethod
    def _get_msg_body(message: ServiceBusMessage | CCEventMessage) -> str:
        body = message.body
        if isinstance(body, str):
            return body
        if isinstance(body, bytes):
            return body.decode("utf-8")
        return str(body)