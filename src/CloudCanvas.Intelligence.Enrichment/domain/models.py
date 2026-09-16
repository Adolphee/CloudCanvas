from dataclasses import dataclass

@dataclass
class ImageTag:
    name: str
    confidence: float

@dataclass
class Photo:
    id: str
    user_id: str
    url: str
    tags: list[ImageTag]
    caption: str
