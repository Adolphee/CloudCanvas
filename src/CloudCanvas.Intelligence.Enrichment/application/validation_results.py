
from dataclasses import dataclass, field
from typing import Any
from domain.models import Photo


@dataclass(slots=True, kw_only=True)
class ValidationResult:
    errors: dict[str, str] = field(default_factory=dict)
    is_valid: bool = field(default=True)
    message: str = field(default="Successful")

@dataclass(slots=True, kw_only=True)
class PhotoValidationResult(ValidationResult):
    valid_photo: Photo | None = field(default=None)

@dataclass(slots=True, kw_only=True)
class EnrichmentPayloadValidationResult(ValidationResult):
    valid_photo: Photo | None = field(default=None)