/// <summary>
/// Simple class to safely hold constant string values.
/// </summary>
namespace CloudCanvas.Application.Common.Constants
{
    public abstract class Unknown
    {
        public const string Username = "unknown_user";
        public const string DisplayName = "Unknown User";
    }
    public abstract class AppSettings
    {
        public const string MaxMessageLength = "MAX_MSG_LEN";
        public const string ProjectionDbName = "PROJECTION_DB_NAME";
        public const string AppInsights = "APPINSIGHTS_INSTRUMENTATIONKEY";
        public const string AppInsightsConnectionString = "APPINSIGHTS_CONNECTIONSTRING";
        public const string AppInsightsRoleName = "APPINSIGHTS_ROLE_NAME";
        public const string AppInsightsRoleInstance = "APPINSIGHTS_ROLE_INSTANCE";
    }

    public abstract class BStorage
    {
        public const string Self = "AzureBlobStorage";
        public const string Uri = "BlobStorageUri";
        public const string ManagedIdentity = "BSManagedIdentity";
        public const string BSConnection = "BSConnectionString";

        public abstract class Meta
        {
            public const string Identifier = "identifier";
            public const string OriginalFilename = "originalFilename";
            public static string UploadedBy = "uploadedBy";
            public static string CompletedOn = "completedOn";
            public static string CreatedOn = "createdOn";
            public const string DeletedOn = "deletedOn";
            public const string Container = "container";

        }

        public abstract class Containers
        {
            public const string PhotoGallery = "photogallery";
            public const string Uploads = "uploads";
            public const string ImgConversions = "imgconversions";
            public const string Thumbnails = "thumbnails";
        }
    }

    public abstract class ServiceBus
    {
        public const string Uri = "ServiceBusUri";
        public const string ManagedIdentity = "SBManagedIdentity";
        public abstract class Topics
        {
            public const string FileUpdates = "file-updates";
        }

        public abstract class Props
        {
            public const string EventType = "eventType";
            public const string ThumbnailSize = "thumbnailSize";
            public const string ContainerName = "ContainerName";
            public const string Operation = "Operation";
        }

        public abstract class Subs
        {
            public const string ExtractMetaData = "extract-metadata";
            public const string CreateThumbnail = "create-thumbail";
            public const string ResizeImage = "resize-image";
            public const string PersistMetadata = "persist-metadata";
        }

        public abstract class Ops
        {
            public const string Tag = "tag-photo";
            public const string Caption = "caption-photo";
        }

        public abstract class Status
        {
            public const string NewBlobDetected = "New Photo Detected";
            public const string ThumbnailCreated = "Thumbnail Created";
            public const string ImageResized = "Image Resized";
            public const string MetadataPersisted = "Metadata Persisted";
            public const string Intelligence = "intelligence";
            public const string OrchestrationFinished = "Thumbnail Orchestration Concluded. Metadata updated.";
        }
    }

    public abstract class Projection
    {
        public const string Sql = "cloudcosmos_sql";
        public const string Uri = "CosmosEndpointURI";
        
        public abstract class Containers {
            public const string UserPhotos = "user_photos";
            public const string Galleries = "galleries";
        }
    }

    public abstract class SQLServer
    {
        public const string ConnectionString = "sqlserver";
    }

    public abstract class Secrets
    {
        public const string MNSTRG = "MNSTRG";
        public const string MTSTRG = "MTSTRG";
        public const string FUMSGO = "FUMSGO";
        public const string FUMSGI = "FUMSGI";
    }

    public abstract class CCClaimTypes
    {
        public const string ObjectIdentfier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Name = "name";
    }
}