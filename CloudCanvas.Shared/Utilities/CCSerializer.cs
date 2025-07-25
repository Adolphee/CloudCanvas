using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.DTOs;
using System.Text.Json;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Constants;

namespace CloudCanvas.Shared.Utilities
{
    public static class CCSerializer
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Converts the provided blob properties and metadata into a <see cref="BlobMetaDTO"/> object.
        /// </summary>
        /// <remarks>This method maps the properties of a blob, as represented by <see
        /// cref="BlobProperties"/>,  to a <see cref="BlobMetaDTO"/> object for further processing or use in the
        /// application.</remarks>
        /// <param name="originalFileName">The original name of the file before it was uploaded to the blob storage.</param>
        /// <param name="blobUrl">The URL of the blob in the storage system.</param>
        /// <param name="props">The properties of the blob, including metadata, content details, and versioning information.</param>
        /// <returns>A <see cref="BlobMetaDTO"/> object containing metadata and properties of the blob, such as its URL, 
        /// original file name, content details, and other relevant attributes.</returns>
        public static BlobMetaDTO MetaFromBlobProperties(string originalFileName, string blobUrl, BlobProperties props)
        {
            Validate.StringValue(nameof(originalFileName), originalFileName, $"Missing {nameof(originalFileName)} and/or {nameof(blobUrl)}. Unable to link metadata to blob.");
            return new BlobMetaDTO
            {
                UserId = Guid.NewGuid().ToString(), // This is just a placeholder for when I introduce auth
                Url = blobUrl,
                OriginalFileName = originalFileName,
                CreatedOn = props.CreatedOn,
                ContainerName = BlobStorage.Containers.Uploads,
                ProcessingStage = (int) BlobProcessingStage.ExtractMetadata,
                Metadata = props.Metadata,
                Thumbnails = new Dictionary<ThumbnailSize, string>(),
                Name = originalFileName,
                Description = String.Empty, // future A.I. implementation will further process and fill in this description
                Tags = new List<string>(), // future A.I. implementation will further process and fill in these tags
                TagCount = props.TagCount,
                BlobType = props.BlobType,
                ETag = props.ETag.ToString().Trim('\"'),
                LastAccessed = props.LastAccessed,
                LastModified = props.LastModified,
                ExpiresOn = props.ExpiresOn,
                ContentType = props.ContentType,
                ContentLength = props.ContentLength,
                ContentLanguage = props.ContentLanguage,
                ContentEncoding = props.ContentEncoding,
                ContentDisposition = props.ContentDisposition,
                CopyId = props.CopyId,
                CopyStatus = props.CopyStatus,
                CopyProgress = props.CopyProgress,
                CopySourceUrl = props.CopySource?.ToString() ?? String.Empty,
                CopyCompletedOn = props.CopyCompletedOn,
                IsLatestVersion = props.IsLatestVersion,
                VersionId = props.VersionId,
            };  
        }

        public static string Serialize<T>(T target) => JsonSerializer.Serialize(Validate.Object(target), _options);
        public static T MetaFromBinaryData<T>(BinaryData blobMetadataDto) => Deserialize<T>(blobMetadataDto.ToString());
        
        /// <summary>
        /// Tries to deserialize a structured string into an object of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the object to try and create.</typeparam>
        /// <param name="blobMetadataDto"></param>
        /// <returns></returns>
        /// <exception cref="CCSerializationException">When the operation fails</exception>
        public static T Deserialize<T>(string blobMetadataDto)
        {
            try
            {
                Validate.StringValue(nameof(blobMetadataDto), blobMetadataDto);
                T? dto = JsonSerializer.Deserialize<T>(blobMetadataDto, _options);
                return Validate.Object(dto);
            }
            catch (InvalidArgumentException e)
            {
                throw new CCSerializationException($"Invalid argument '{nameof(blobMetadataDto)}' provided with value: '{blobMetadataDto}'.", e);
            }
        }
    }
}
