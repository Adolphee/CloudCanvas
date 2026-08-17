from dataclasses import dataclass

@dataclass
class ImageTag:
    name: str
    confidence: float

@dataclass
class Photo:
    id: str
    url: str
    user_id: str
    tags: list[ImageTag]
    caption: str
