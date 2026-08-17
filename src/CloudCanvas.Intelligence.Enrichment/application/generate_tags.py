# use case orchestration
    
###
# Generate image enrichment ((WIP))
# 1. Validate the incoming event. --> should 
# 2. Ask ImageAnalyzer for tags and caption.
# 3. Apply confidence and moderation policy. --> ?
# 4. Persist enrichment state.
# 5. Publish ImageEnriched (event, messaging).
# 6. Make processing idempotent.
###

from application.ports.image_analyzer import ImageAnalyzer
from domain.models import ImageTag

async def generate_tags(client: ImageAnalyzer, image_url: str) -> list[ImageTag]:
    if image_url is None or image_url.strip() == "":
        raise ValueError("Image URL is empty or None.")
    return await client.generate_tags(image_url)

