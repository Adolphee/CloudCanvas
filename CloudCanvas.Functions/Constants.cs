
using CloudCanvas.Constants;

/// <summary>
/// Simple class to safely hold constant string values.
/// </summary>
namespace CloudCanvas.Functions.Constants
{
    /// <summary>
    /// Represents constants and nested types related to Azure Blob Storage configuration.
    /// </summary>
    /// <remarks>AzFunc class provides predefined constants for common Azure Blob Storage settings, such as
    /// connection strings and container names. It also includes a nested class, <see cref="Containers"/>, which defines
    /// container-specific constants.</remarks>
    public abstract class WebJobs: BlobStorage
    {
        public const string AzureWebJobsStorage = "AzureWebJobsStorage";
    }


    // Meant to be in a different azure function, intended to write metadata to azue storage

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