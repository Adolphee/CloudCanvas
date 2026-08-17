import logging
from application.ports.image_analyzer import ImageAnalyzer

async def generate_caption(client: ImageAnalyzer, image_url: str) -> str:
    logging.info(f"Generating caption for the image URL: {image_url}")
    if image_url is None or image_url.strip() == "":
        logging.exception("Image URL is empty or None.")
        raise
    try: return await client.generate_caption(image_url)
    except Exception as e:
        logging.exception(f"Failed to generate caption. Image: {image_url}", e)
        raise