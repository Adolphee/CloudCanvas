
/// <summary>
/// Simple class to safely hold constant string values.
/// </summary>
namespace CloudCanvas.Constants
{
    /// <summary>
    /// Represents constants and nested types related to Azure Blob Storage configuration.
    /// </summary>
    /// <remarks>This class provides predefined constants for common Azure Blob Storage settings, such as
    /// connection strings and container names. It also includes a nested class, <see cref="Containers"/>, which defines
    /// container-specific constants.</remarks>
    public abstract class AzureBlobStorage
    {
        public const string Self = "AzureBlobStorage";
        public const string ConnectionString = "ConnectionString";
        public const string ContainerName = "ContainerName";

        public abstract class Containers
        {
            public const string PhotoGallery = "photogallery";
            public const string Uploads = "uploads";
            public const string ImgConversions = "imgconversions";
        }

        // Meant to be in a different azure function, intended to write metadata to azue storage
        public abstract class BlobMeta
        {
            public const string OriginalFileName = "OriginalFileName";
            public const string OriginalImageFormat = "OriginalImageFormat";
            public const string ContentType = "ContentType";
            public const string UploadedBy = "UploadedBy";
            public const string Project = "Project";
            public const string ProcessingStage = "ProcessingStage";
        }
    }
}