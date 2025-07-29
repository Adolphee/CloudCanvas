
/// <summary>
/// Simple class to safely hold constant string values.
/// </summary>
namespace CloudCanvas.Shared.Constants
{
    /// <summary>
    /// Represents constants and nested types related to Azure Blob Storage configuration.
    /// </summary>
    /// <remarks>This class provides predefined constants for common Azure Blob Storage settings, such as
    /// connection strings and container names. It also includes a nested class, <see cref="Containers"/>, which defines
    /// container-specific constants.</remarks>
    public abstract class BlobStorage
    {
        public const string Self = "AzureBlobStorage";

        public abstract class Meta
        {
            public const string Identifier = "identifier";
            public const string OriginalfileName = "originalFilename";
            public static string UploadedBy = "uploadedBy";
            public const string DeletedOn = "deletedOn";
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
        public abstract class Topics
        {
            public const string FileUpdates = "file-updates";
        }

        public abstract class Props
        {
            public const string EventType = "eventType";
            public const string ThumbnailSize = "thumbnailSize";
        }

        public abstract class Subs
        {
            public const string ExtractMetaData = "extract-metadata";
            public const string CreateThumbnail = "create-thumbail";
            public const string ResizeImage = "resize-image";
            public const string PersistMetadata = "persist-metadata";
        }

        public abstract class Status
        {
            public const string MetadataExctracted = "Metadata Exctracted";
            public const string ThumbnailCreated = "Thumbnail Created";
            public const string ImageResized = "Image Resized";
            public const string MetadataPersisted = "Metadata Persisted";
        }
    }

    public abstract class CloudCosmos
    {
        public const string Sql = "cloudcosmos_sql";
        public abstract class Containers {
            public const string BlobMeta = "blob_metadata";
        }
    }

    public abstract class Secrets
    {
        public const string MNSTRG = "MNSTRG";
        public const string MTSTRG = "MTSTRG";
        public const string FUMSGO = "FUMSGO";
        public const string FUMSGI = "FUMSGI";
    }

    public abstract class Config
    {
        public const string MaxMessageLength = "MAX_MSG_LEN";
    }
}