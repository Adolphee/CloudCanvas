
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
    public abstract class Func: AzureBlobStorage
    {
        public const string AzureWebJobsStorage = "AzureWebJobsStorage";
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

    public abstract class FileTypes
    {
        public abstract class Image
        {
            public const string Self = "image";
            public const string PNG = $"{Self}/png";
            public const string JPEG = $"{Self}/jpeg";
            public const string JPG = $"{Self}/jpg";
        }
    }
}