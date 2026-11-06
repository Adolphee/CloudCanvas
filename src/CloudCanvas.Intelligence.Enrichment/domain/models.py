from dataclasses import dataclass
import json
from typing import Any
from domain.constants import Constants

A = Constants.Attr
@dataclass
class ImageTag:
    name: str
    confidence: float | None
    def __init__(self, name: str, confidence: float | None = None):
        self.name = name
        self.confidence = confidence
    def toJSON(self): return { A.NAME: self.name, A.CONFIDENCE: self.confidence }

@dataclass(kw_only=True, slots=True)
class Photo:
    id: str
    user_id: str
    url: str
    caption: str
    tags: list[ImageTag]

@dataclass
class CCEventMessage:
    id: str
    subject: str
    content_type: str
    correlation_id: str
    session_id: str
    body: str | object
    properties: dict[str | bytes, Any]

@dataclass
class PhotoVerificationResult:
    is_completed: bool
    photo: Photo | None