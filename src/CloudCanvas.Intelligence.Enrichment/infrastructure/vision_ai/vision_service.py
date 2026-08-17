import logging
from domain.models import ImageTag
from application.ports.image_analyzer import ImageAnalyzer
from azure.ai.vision.imageanalysis.models import VisualFeatures
from azure.ai.vision.imageanalysis.aio import ImageAnalysisClient

class VisionService(ImageAnalyzer):
    def __init__(self, client: ImageAnalysisClient): self.client = client

    async def generate_tags(self, image_url: str) -> list[ImageTag]:
        try:
            result = await self.client.analyze_from_url(image_url,[VisualFeatures.TAGS])
            if result.tags is not None:
                logging.info(f"{len(result.tags)} tags generated A.I.")
                return [ImageTag(tag.name, tag.confidence) for tag in result.tags.list]
            logging.warning("Azure Vision returned no caption for image URL %s", image_url)
            return []
        except Exception as e:
            logging.exception("Azure Vision analysis failed. Image: %i", image_url)
            raise

    async def generate_caption(self, image_url: str) -> str:
        try:
            result = await self.client.analyze_from_url(image_url,[VisualFeatures.CAPTION])
            if result.caption is not None:
                logging.info("Generated caption: %s", result.caption)
                return result.caption.text
            logging.warning("No caption was generated. Should be investigated.")
            return ""
        except Exception as e:
            logging.exception("Azure Vision analysis failed. Image: %s", image_url)
            raise
