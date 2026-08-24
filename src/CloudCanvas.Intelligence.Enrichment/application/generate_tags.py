# use case orchestration
    
###
# Generate image enrichment ((WIP))
# 1. Validate the incoming event. --> should 
# 2. Ask ImageAnalyzer for tags and caption.
# 3. Apply confidence and moderation policy. --> !
# 4. Persist enriched state.
# 5. Publish ImageEnriched (event, messaging). --> WIP
# 6. Make processing idempotent.
###
from application.exceptions import ImageUrlNotFoundException
from application.ports.image_analyzer import ImageAnalyzer
from domain.models import ImageTag

async def generate_tags(client: ImageAnalyzer, image_url: str) -> list[ImageTag]:
    if not image_url or not image_url.strip():
        raise ImageUrlNotFoundException("Image URL is empty or None.")
    return await client.generate_tags(image_url)

