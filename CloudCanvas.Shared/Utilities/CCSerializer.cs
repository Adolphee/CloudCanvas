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
        /// <param name="identifier">The original name of the file before it was uploaded to the blob storage.</param>
        /// <param name="blobUrl">The URL of the blob in the storage system.</param>
        /// <param name="props">The properties of the blob, including metadata, content details, and versioning information.</param>
        /// <returns>A <see cref="BlobMetaDTO"/> object containing metadata and properties of the blob, such as its URL, 
        /// original file name, content details, and other relevant attributes.</returns>
        public static BlobMetaDTO MetaFromBlobProperties(string identifier, string blobUrl, BlobProperties props)
        {
            Validate.StringValue(nameof(identifier), identifier, $"Missing {nameof(identifier)} and/or {nameof(blobUrl)}. Unable to link metadata to blob.");
            bool deleted = false;
            DateTimeOffset result = DateTimeOffset.MinValue;
                if (props.Metadata.Keys.Contains(BlobStorage.Meta.DeletedOn))
                {
                    try
                    {
                        deleted = DateTimeOffset.TryParse(props.Metadata[BlobStorage.Meta.DeletedOn], out result);
                    }
                    catch (Exception) {}  // swallowing this because it tells us deletedOn wasn't set so we can proceed as planned
                }
           
            return new BlobMetaDTO
            {
                Id = identifier,
                UserId = props.Metadata[BlobStorage.Meta.UploadedBy], // This is just a placeholder for when I introduce auth
                Url = blobUrl,
                OriginalFilename = props.Metadata[BlobStorage.Meta.OriginalfileName],
                CreatedOn = props.CreatedOn,
                ContainerName = BlobStorage.Containers.Uploads,
                ProcessingStage = (int)BlobProcessingStage.UploadSuccessful,
                Metadata = props.Metadata,
                Thumbnails = new Dictionary<ThumbnailSize, string>(),
                Name = identifier,
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
                DeletedOn = deleted? result: null //only assign 'result' when it has been altered succesfully and deleted = true
            };  
        }

        public static string Serialize<T>(T target) => JsonSerializer.Serialize(Validate.Object(target), _options);
        public static T MetaFromBinaryData<T>(BinaryData blobMetadataDto) => Deserialize<T>(blobMetadataDto.ToString());
        
        /// <summary>
        /// Tries to deserialize a structured string into an object of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the object to try and create.</typeparam>
        /// <param name="blobMetadataJson"></param>
        /// <returns></returns>
        /// <exception cref="CCSerializationException">When the operation fails</exception>
        public static T Deserialize<T>(string blobMetadataJson)
        {
            try
            {
                Validate.StringValue(nameof(blobMetadataJson), blobMetadataJson);
                T? dto = JsonSerializer.Deserialize<T>(blobMetadataJson, _options);
                return Validate.Object(dto);
            }
            catch (InvalidArgumentException e)
            {
                throw new CCSerializationException($"Invalid argument '{nameof(blobMetadataJson)}' provided with value: '{blobMetadataJson}'.", e);
            }
        }
    }
}
