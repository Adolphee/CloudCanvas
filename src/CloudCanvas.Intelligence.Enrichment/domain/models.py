from dataclasses import dataclass
from typing import Any

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

@dataclass
class CCEventMessage:
    id: str
    subject: str
    content_type: str
    correlation_id: str
    session_id: str
    properties: dict[str | bytes, Any]
    body: str | object

@dataclass
class PhotoVerificationResult:
    is_complered: bool
    photo: Photo | None