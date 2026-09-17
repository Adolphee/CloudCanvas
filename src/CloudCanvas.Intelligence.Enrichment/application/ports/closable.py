
from typing import Protocol

class Closable(Protocol):
    async def close_connection(self): ...