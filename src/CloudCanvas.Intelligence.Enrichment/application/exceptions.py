from dataclasses import dataclass, field

@dataclass(slots=True, kw_only=True)
class BaseEnrichmentException(Exception):
    msg: str
    operation: str

@dataclass(slots=True, kw_only=True)
class SmartTagFailedException(BaseEnrichmentException): 
    operation: str = field(default="tag_photo")

@dataclass(slots=True)
class SmartCaptionFailedException(BaseEnrichmentException): ...

@dataclass(slots=True)
class ImageUrlNotFoundException(BaseEnrichmentException):
    message: str = field(default="Image URL not found.")
    operation: str = field(default="caption_photo")

@dataclass(frozen=True, slots=True)
class ImageIdNotFoundException(Exception):
    msg: str = field(default="Image id not found.")

@dataclass(frozen=True, slots=True)
class ImageUserIdNotFoundException(Exception):
    message: str = field(default="user_id not found.")


@dataclass(frozen=True, slots=True, kw_only=True)
class AppSettingsNotFoundException(Exception):
    setting_name: str
    message: str = field(default="Environment variable not found.")

@dataclass(slots=True)
class ValidationException(Exception):
    argument: str = field()
    value_given: str = field()
    message: str  = field(default="A validation exception occurred.")

@dataclass(slots=True, kw_only=True)
class BadRequestException(Exception):
    errors: dict[str, str] = field(default_factory=dict) 

@dataclass(kw_only=True)
class InvalidPayloadException(Exception): ...

