import os, logging as logger
from domain.constants import Constants
from azure.cosmos.aio import CosmosClient
from sqlalchemy.engine import create_engine, URL
from azure.ai.vision.imageanalysis.aio import ImageAnalysisClient
from azure.identity.aio import DefaultAzureCredential
from azure.servicebus.aio import ServiceBusClient
from application.ports import ImageAnalyzer, ProjectionService, Messenger
from application.exceptions import AppSettingsNotFoundException
from infrastructure.mssql_service import SQLService
from infrastructure.servicebus import ServiceBusService
from infrastructure.vision_ai.vision_service import VisionService
from infrastructure.cosmos_db.cosmos_service import CosmosService

def build_image_analyzer(credential: DefaultAzureCredential) -> ImageAnalyzer:
    msg = "VISION_ENDPOINT and VISION_KEY must be set in environment variables."
    try:
        vision_endp = os.getenv(Constants.AppSettings.VISION_ENDPOINT)
        vision_key = os.getenv(Constants.AppSettings.VISION_KEY)
        if not vision_endp or not vision_key:
            logger.exception(msg)
            raise AppSettingsNotFoundException(setting_name="VISION_KEY", message=msg)
        client = ImageAnalysisClient(vision_endp, credential)
        return VisionService(client)
    except Exception as e:
        logger.exception("An exception occurred during initialization of VisionService:\n%a", e)
        raise AppSettingsNotFoundException(setting_name="VISION_KEY and/or VISION_ENDPOINT", message=msg)from e

def build_projection_service(credential: DefaultAzureCredential) -> ProjectionService:
    msg = "COSMOS secrets must be set in environment variables."
    ep_name = Constants.AppSettings.COSMOS_ENDPOINT
    try: 
        ENDPOINT = os.getenv(ep_name)
        if not ENDPOINT: 
            logger.exception(msg)
            raise AppSettingsNotFoundException(setting_name=ep_name, message=msg)
        client = CosmosClient(ENDPOINT, credential)
        return CosmosService(client)
    except Exception as e: 
        logger.exception("An exception occurred during initialization of CosmosService:\n%a", e)
        logger.error(msg)
        raise AppSettingsNotFoundException(setting_name=ep_name, message=msg) from e
    
def build_messenger(credential: DefaultAzureCredential) -> Messenger:
    key_name = Constants.AppSettings.SB_ENDPOINT
    msg = f"{key_name} must be set in environment variables."
    SB_ENDPOINT = str(os.environ.get(key_name))
    if not SB_ENDPOINT: raise AppSettingsNotFoundException(setting_name=key_name, message=msg)
    client = ServiceBusClient(fully_qualified_namespace=SB_ENDPOINT, credential=credential)
    return ServiceBusService(client)

def build_persistence_store() -> SQLService:
    conn_str = URL.create(
        "mssql+pymssql",
        username="sa",
        password="CloudCanvas#2026",
        host="sqlserver",
        port=1433,
        database="CloudCanvas.Identity",
        query={},
    )
    engine = create_engine(conn_str)
    return SQLService(engine)