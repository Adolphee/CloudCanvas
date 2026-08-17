import os
import logging
from azure.ai.vision.imageanalysis.aio import ImageAnalysisClient
from azure.core.credentials import AzureKeyCredential
from application.ports.image_analyzer import ImageAnalyzer
from infrastructure.vision_ai.vision_service import VisionService


def build_image_analyzer() -> ImageAnalyzer:
    try:
        VISION_ENDPOINT = str(os.environ.get("VISION_ENDPOINT"))
        VISION_KEY = str(os.environ.get("VISION_KEY"))
        if not VISION_ENDPOINT or not VISION_KEY:
            raise ValueError("VISION_ENDPOINT and VISION_KEY must be set in environment variables.")
    except Exception as e:
        logging.exception("An exception occurred during initialization of ImageAnalyzer.", {e})
        logging.error("Set them before running this sample.")
        raise
    client = ImageAnalysisClient(
        endpoint=os.environ["VISION_ENDPOINT"],
        credential=AzureKeyCredential(os.environ["VISION_KEY"]),
    )
    return VisionService(client)