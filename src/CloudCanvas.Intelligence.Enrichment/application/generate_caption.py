import logging
from application.exceptions import ImageUrlNotFoundException, SmartTagFailedException
from application.ports.image_analyzer import ImageAnalyzer

async def generate_caption(client: ImageAnalyzer, image_url: str) -> str:
    logging.info(f"Generating caption for the image URL: {image_url}")
    
    try: return await client.generate_caption(image_url)
    except Exception as e:
        err_msg = f"Failed to generate caption. Image: {image_url}"
        logging.exception(err_msg)
        raise SmartTagFailedException(msg=err_msg) from e