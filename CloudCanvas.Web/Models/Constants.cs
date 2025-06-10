
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
        public const string AzureWebJobsStorage = "AzureWebJobsStorage";
        public const string ConnectionString = "ConnectionString";
        public const string ContainerName = "ContainerName";

        public abstract class Containers
        {
            public const string PhotoGallery = "photogallery";
            public const string Uploads = "uploads";
            public const string ImgConversions = "imgconversions";
        }
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