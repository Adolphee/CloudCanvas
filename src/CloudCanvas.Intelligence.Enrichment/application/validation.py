
import json, logging as logger
from validators import ValidationError, uuid, url
from domain.models import Photo
from application.validation_results import PhotoValidationResult
from application.exceptions import BadRequestException, InvalidPayloadException, ValidationException, ImageUrlNotFoundException

class Validator:
    @staticmethod
    def validate_enrichment_request(payload: bytes) -> Photo:
        valid_payload = Validator.validate_photo_payload(payload)
        res = Validator.validate_photo(valid_payload)
        if not res.is_valid or not res.valid_photo:
            exc = BadRequestException()
            for (k, v) in res.errors.items(): 
                logger.error(f"Validation error for {k}: {v}")
            raise exc
        return res.valid_photo
        
    @staticmethod
    def validate_photo_payload(payload: bytes) -> Photo:
        photo: Photo
        try:
            body = json.loads(payload.decode()) 
            photo = Photo(**body)
            return photo
        except Exception as e:
            err_msg = "Invalid message payload."
            logger.exception(err_msg)
            raise InvalidPayloadException() from e

    @staticmethod
    def validate_photo(photo: Photo) -> PhotoValidationResult:
        errors = {}
        result = PhotoValidationResult()

        validation = {
            "id": lambda: uuid(photo.id),
            "user_id": lambda: uuid(photo.user_id),
            "url": lambda: url(photo.url)
        }

        for key, func in validation.items():
            try: func()
            except ValidationError:
                errors[key] = f"A valid {key} is required."
                logger.exception(errors[key])

        result.is_valid = len(errors) == 0
        if result.is_valid:
            result.valid_photo = photo
            result.message = "Valid photo object."
        else: 
            result.valid_photo = None
            result.errors = errors
            result.message = "Invalid photo object."
        return result