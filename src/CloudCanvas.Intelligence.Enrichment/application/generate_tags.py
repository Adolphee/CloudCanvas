
from application.ports.image_analyzer import ImageAnalyzer
from domain.models import ImageTag

async def generate_tags(client: ImageAnalyzer, image_url: str) -> list[ImageTag]:
    return await client.generate_tags(image_url)

