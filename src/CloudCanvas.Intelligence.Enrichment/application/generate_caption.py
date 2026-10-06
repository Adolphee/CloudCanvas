import logging
from application.exceptions import SmartTagFailedException
from application.ports.image_analyzer import ImageAnalyzer

async def generate_caption(client: ImageAnalyzer, image_url: str) -> str:
    logging.info("Generating caption for the image URL: %s", image_url)
    try: return await client.generate_caption(image_url)
    except Exception as e:
        err_msg = "Failed to generate caption. Image: %s"
        logging.exception(err_msg, image_url)
        raise SmartTagFailedException(msg=err_msg % image_url) from e