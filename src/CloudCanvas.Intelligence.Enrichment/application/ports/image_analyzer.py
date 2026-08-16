from typing import Protocol
from domain.image_tag import ImageTag


class ImageAnalyzer(Protocol):
    async def generate_tags(self, image_url: str) -> list[ImageTag]: ...
    async def generate_caption(self, image_url: str) -> str: ...
    
