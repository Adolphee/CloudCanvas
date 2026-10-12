import json
import logging as logger
from unittest import result
from domain.constants import Constants
from sqlalchemy import Engine, MetaData, Table, select, update
from sqlalchemy.exc import NoSuchTableError
from application.exceptions import TableNotFoundException
from application.ports import PersistenceService


class SQLService(PersistenceService):
    def __init__(self, engine: Engine): self._engine = engine

    def get_table(self, name: str) -> Table:
        metadata = MetaData()
        try:
            return Table(name, metadata, autoload_with=self._engine)
        except Exception | NoSuchTableError as e:
            logger.exception("Table %a not found.", name)
            raise TableNotFoundException(
                tableName=name, database=self._engine.url.database or "[Unknown / Missing]")

    def update_smartCaption(self, photo_id: str, ai_caption: str):
        table = self.get_table(Constants.Tables.PHOTOS)
        with self._engine.begin() as connection:
            try:
                connection.execute(
                    table.update()
                    .where(table.c.Id == photo_id)
                    .values(SmartCaption=ai_caption)
                )
                connection.commit()
            except Exception | NoSuchTableError as e:
                logger.exception("Failed to save caption to persistence store (SQL): %a", photo_id)
            finally:
                connection.close()

    def update_smartTags(self, photo_id: str, ai_tags: list[str]):
        table = self.get_table(Constants.Tables.PHOTOS)
        with self._engine.begin() as connection:
            try:
                connection.execute(
                    table.update()
                    .where(table.c.Id == photo_id)
                    .values(SmartTags=json.dumps(ai_tags))
                )
                connection.commit()
            except Exception as e:
                logger.exception("Failed to save caption to persistence store (SQL): %a", photo_id)
                raise
            finally:
                connection.close()

    def verify_no_prior_enrichment(self, photo_id: str) -> bool:
        table = self.get_table(Constants.Tables.PHOTOS)
        with self._engine.begin() as conn:
            try:
                query = select(table.c.SmartTags, table.c.SmartCaption).where( table.c.Id == photo_id).limit(1)
                row = conn.execute(query).fetchone()
                if row and row.SmartTags and row.SmartCaption:
                    logger.info("Prior enrichment found for photo: %a", row._asdict())
                    return True
            except Exception as e:
                logger.exception("Failed to verify prior enrichment in persistence store for photo: %a", photo_id)
                return False
        return False

    def close_connection(self):
        if self._engine: self._engine.dispose()
