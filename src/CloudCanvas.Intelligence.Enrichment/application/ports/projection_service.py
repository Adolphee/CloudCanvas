from typing import Any

from application.ports import Closable
from azure.cosmos.partition_key import PartitionKeyType
from domain.models import Photo

class ProjectionService(Closable):
    async def project_tags(self, image: Photo) -> bool: ...
    async def project_caption(self, image: Photo) -> bool: ...
    async def verify_no_prior_enrichment(self, photo_id: str, user_id: PartitionKeyType) ->  Photo | None: ...