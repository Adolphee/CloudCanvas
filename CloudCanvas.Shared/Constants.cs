
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
        public const string ConnectionString = "ConnectionString";
        public const string ContainerName = "ContainerName";

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

        public const string Self = "AzureServiceBus";
        /// <summary>
        /// Constructs a formatted string representing an event's details.
        /// </summary>
        /// <param name="mainEvent">The name or identifier of the main event.</param>
        /// <param name="processingStage">The current stage of processing for the event.</param>
        /// <param name="status">The status of the event at the specified processing stage: "done" or "start".</param>
        /// <returns>A string in the format "mainEvent--processingStage--status" representing the event's details.</returns>
        public static string GetRealEventString(string mainEvent, string processingStage, string status) => $"{mainEvent}--{processingStage}--{status}";
        public abstract class Topics
        {
            public const string FileUpdates = "file-updates";
            public abstract class FileUpdate {
                public const string Send = $"{FileUpdates}-send";
                public const string Listen = $"{FileUpdates}-listen";
            }
        }

        public abstract class Props
        {
            public const string EventType = "eventType";
        }

        public abstract class Subs
        {
            public const string ExtractMetaData = "extract-metadata";
            public const string CreateThumbnail = "create-thumbail";
            public const string ResizeImage = "resize-image";
            public const string PersistMetadata = "persist-metadata";
        }
    }

    public abstract class CloudCosmos
    {
        public const string Sql = "cloudcosmos_sql";
        public abstract class Containers {
            public const string BlobMeta = "blob_metadata";
        }
    }
}