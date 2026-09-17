from application.ports import Closable
from domain.models import CCEventMessage, Photo

class Messenger(Closable):
    async def send_message(self, queue_name: str, message: CCEventMessage): ...
    async def notify_enrichment_complete(self, photo: Photo): ...
    