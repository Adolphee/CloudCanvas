
class Constants:
    class AppSettings:
        PROJECTION_DB_NAME = "PROJECTION_DB_NAME"
        ENV_VARS="environmentVariables"
        SB_TOPIC="SB_TOPIC"
        SBSUB_TAG="SBSUB_TAG"
        SBSUB_CAP="SBSUB_CAP"
        SB_CONN="SB_CONN"
        PROJ_PHOTOS_CONTAINER="PROJ_PHOTOS_CONTAINER"
    class Attr:
        ID="id" 
        USER_ID="userId"
        TAGS="smartTags"
        CAPTION="smartCaption"
        URL="location"
        PHOTO="photo",
        CONFIDENCE="confidence"
        NAME="name"
    class Status:
        COMPLETE="enrichment_complete"
    class Services:
        ANALYZER="analyzer"
        PROJECTOR="projectior"
        MESSENGER="messanger"
    class Tables:
        PHOTOS="Photos"