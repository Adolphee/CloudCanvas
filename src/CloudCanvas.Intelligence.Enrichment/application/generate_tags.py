# use case orchestration

import os
import logging
from azure.ai.vision.imageanalysis.aio import ImageAnalysisClient
from azure.ai.vision.imageanalysis.models import VisualFeatures
from azure.core.credentials import AzureKeyCredential

logging.basicConfig(level=logging.INFO)

try:
    VISION_ENDPOINT = str(os.environ.get("VISION_ENDPOINT"))
    VISION_KEY = str(os.environ.get("VISION_KEY"))
    if not VISION_ENDPOINT or not VISION_KEY:
        raise ValueError("VISION_ENDPOINT and VISION_KEY must be set in environment variables.")
except Exception as e:
    logging.error("Missing environment variable 'VISION_ENDPOINT' or 'VISION_KEY'")
    logging.error("Set them before running this sample.")
    exit()


async def generate_tags(image_url: str):
    logging.info(f"Generating tags for the image URL: {image_url}")
    if image_url is None or image_url.strip() == "":
        logging.error("Image URL is empty or None.")
        return []

    client = ImageAnalysisClient(endpoint=VISION_ENDPOINT, credential=AzureKeyCredential(VISION_KEY))
    try:
        result = await client.analyze_from_url(
            image_url=image_url,
            visual_features=[VisualFeatures.TAGS],
            gender_neutral_caption=True,
            language="en" 
        )
        
        if result.tags is not None:
            logging.info(f"{result.tags.__sizeof__()} tags found in the analysis result.")
            tags = [tag.name for tag in result.tags.list]
            return tags

        logging.info("No tags found in the analysis result.")
        return []
    except Exception as e:
        logging.error(f"Error analyzing image: {e}")
        return []
    finally:
        await client.close()