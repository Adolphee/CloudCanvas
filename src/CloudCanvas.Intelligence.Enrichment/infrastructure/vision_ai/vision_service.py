import logging
from azure.ai.vision.imageanalysis.aio import ImageAnalysisClient
from azure.ai.vision.imageanalysis.models import VisualFeatures
from application.ports.image_analyzer import ImageAnalyzer
from domain.image_tag import ImageTag

logging.basicConfig(level=logging.INFO)

class VisionService(ImageAnalyzer):
    def __init__(self, client: ImageAnalysisClient):
        self.client = client

    async def generate_tags(self, image_url: str) -> list[ImageTag]:
        logging.info(f"Generating tags for the image URL: {image_url}")
        if image_url is None or image_url.strip() == "":
            logging.exception("Image URL is empty or None.")
            raise

        try:
            result = await self.client.analyze_from_url(
                image_url=image_url,
                visual_features=[VisualFeatures.TAGS],
                gender_neutral_caption=True,
            )
            if result.tags is not None:
                logging.info(f"{result.tags.__sizeof__()} tags found in the analysis result.")
                tags = [ImageTag(name=tag.name, confidence=tag.confidence) for tag in result.tags.list]
                return tags

            logging.info("No tags found in the analysis result.")
            return []
        except Exception as e:
            logging.exception(f"Azure Vision analysis failed. Image: {e}")
            raise

    async def generate_caption(self, image_url: str) -> str:
        logging.info(f"Generating caption for the image URL: {image_url}")
        if image_url is None or image_url.strip() == "":
            logging.exception("Image URL is empty or None.")
            raise

        try:
            result = await self.client.analyze_from_url(
                image_url=image_url,
                visual_features=[VisualFeatures.CAPTION],
            )
            if result.caption is not None:
                logging.info(f"Generated caption: {result.caption}")
                return str(result.caption)

            logging.info("No caption found in the analysis result.")
            return ""
        except Exception as e:
            logging.exception(f"Azure Vision analysis failed. Image: {e}")
            raise
