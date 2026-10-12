import json, logging

from azure.cosmos import CosmosDict
from application.validation import Validator
from domain.constants import Constants
from domain.models import ImageTag, Photo

class Mapper:
    def __init__(self):
        self.logger = logging.getLogger(__name__)

    @staticmethod
    def to_tag_list(tags_dict: list[CosmosDict]) -> list[ImageTag]:
        A = Constants.Attr
        return [ImageTag(name=tag[A.NAME], confidence=tag[A.CONFIDENCE]) for tag in tags_dict]

    @staticmethod
    def to_photo(photo_dict: CosmosDict) -> Photo | None:
        A = Constants.Attr
        photo = Photo(
            id = str(photo_dict.get(A.ID)),
            user_id= str(photo_dict.get(A.USER_ID)),
            tags = list[ImageTag](),
            url = str(photo_dict.get(A.URL)),
            caption = str(photo_dict.get(A.CAPTION))
        )

        photo = Validator.validate_photo(photo).valid_photo
        
        is_completed = A.TAGS in photo_dict and A.CAPTION in photo_dict
        if photo and is_completed:
            photo.caption = photo_dict[A.CAPTION]
            photo.tags = [ImageTag(name=tag[A.NAME], confidence=tag[A.CONFIDENCE]) for tag in photo_dict[A.TAGS]]
        return photo

#[ImageTag(name=tag[A.NAME], confidence=tag[A.CONFIDENCE]) for tag in photo_dict[A.TAGS]]