import os, logging
from azure.cosmos.aio import CosmosClient
from azure.ai.vision.imageanalysis.aio import ImageAnalysisClient
from azure.identity.aio import DefaultAzureCredential
from azure.servicebus.aio import ServiceBusClient
from application.ports import ImageAnalyzer, ProjectionService, Messenger
from azure.core.credentials import AzureKeyCredential
from application.exceptions import AppSettingsNotFoundException
from infrastructure.servicebus import ServiceBusService
from infrastructure.vision_ai.vision_service import VisionService
from infrastructure.cosmos_db.cosmos_service import CosmosService


async def build_image_analyzer() -> ImageAnalyzer:
    msg = "VISION_ENDPOINT and VISION_KEY must be set in environment variables."
    try:
        VISION_ENDPOINT = os.getenv("VISION_ENDPOINT")
        VISION_KEY = os.getenv("VISION_KEY")
        if not VISION_ENDPOINT or not VISION_KEY:
            logging.exception(msg)
            raise AppSettingsNotFoundException(setting_name="VISION_KEY", message=msg)
        client = ImageAnalysisClient(VISION_ENDPOINT, AzureKeyCredential(VISION_KEY))
        return VisionService(client)
    except Exception as e:
        logging.exception("An exception occurred during initialization of VisionService.", {e})
        logging.error(msg)
        raise AppSettingsNotFoundException(setting_name="VISION_KEY", message=msg)from e

async def build_projection_service() -> ProjectionService:
    msg = "COSMOS secrets must be set in environment variables."
    ep_name = "COSMOS_ENDPOINT"
    key_name = "COSMOS_KEY"
    ssl_verify_setting = "COSMOS_CONN_VERIFY"
    try: 
        ENDPOINT = os.getenv(ep_name)
        COSMOS_CONN_VERIFY = os.getenv(ssl_verify_setting)
        if not ENDPOINT: 
            logging.exception(msg)
            raise AppSettingsNotFoundException(setting_name=key_name, message=msg)
        client = CosmosClient(ENDPOINT, credential=DefaultAzureCredential()) #TODO: Set connection_verify=True before production
        return CosmosService(client)
    except Exception as e: 
        logging.exception("An exception occurred during initialization of CosmosService.", {e})
        logging.error(msg)
        raise AppSettingsNotFoundException(setting_name=ep_name, message=msg) from e
    
async def build_messenger() -> Messenger:
    msg = "SB_ENDPOINT must be set in environment variables."
    key_name = "SB_ENDPOINT"
    SB_ENDPOINT = str(os.environ.get(key_name))
    if not SB_ENDPOINT: raise AppSettingsNotFoundException(setting_name=key_name, message=msg)
    async with ServiceBusClient(fully_qualified_namespace=SB_ENDPOINT, credential=DefaultAzureCredential()) as client: return ServiceBusService(client)