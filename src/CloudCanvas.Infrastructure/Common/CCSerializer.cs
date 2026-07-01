using Azure.Storage.Blobs.Models;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure;
using CloudCanvas.Infrastructure.DTOs;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace CloudCanvas.Infrastructure.Common
{
    public class CCSerializer
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Converts the provided blob properties and metadata into a <see cref="BlobMetadata"/> object.
        /// </summary>
        /// <remarks>This method maps the properties of a blob, as represented by <see
        /// cref="BlobProperties"/>,  to a <see cref="BlobMetadata"/> object for further processing or use in the
        /// application.</remarks>
        /// <param name="identifier">The original name of the file before it was uploaded to the blob storage.</param>
        /// <param name="blobUrl">The URL of the blob in the storage system.</param>
        /// <param name="props">The properties of the blob, including metadata, content details, and versioning information.</param>
        /// <returns>A <see cref="BlobMetadata"/> object containing metadata and properties of the blob, such as its URL, 
        /// original file name, content details, and other relevant attributes.</returns>
        public static BlobMetadata MetaFromBlobProperties(string identifier, string blobUrl, BlobProperties props)
        {
            bool deleted = false;
            DateTimeOffset result = DateTimeOffset.MinValue;
                if (props.Metadata.Keys.Contains(BStorage.Meta.DeletedOn))
                {
                    try
                    {
                        deleted = DateTimeOffset.TryParse(props.Metadata[BStorage.Meta.DeletedOn], out result);
                    }
                    catch (Exception) {}  // swallowing this because it tells us deletedOn wasn't set so we can proceed as planned
                }

            // TODO: uploadedBy / userID will be implemented with the Auth milestone --> Done
            var uploadedBy = props.Metadata.TryGetValue(BStorage.Meta.UploadedBy, out var uploader) ? uploader : null;
            var oFilename = props.Metadata.TryGetValue(BStorage.Meta.OriginalFilename, out var originalFilename) ? originalFilename : identifier;
            var containerName = props.Metadata.TryGetValue(BStorage.Meta.Container, out var container) ? container : BStorage.Containers.Uploads;
            return new BlobMetadata
            {
                Id = identifier,
                UserId = uploadedBy ?? "Unknown User",
                Url = blobUrl,
                OriginalFilename = oFilename,
                CreatedOn = props.CreatedOn,
                ContainerName = containerName,
                ProcessingStage = 0, // TOFIX: this is misleading when this method is not 
                Metadata = props.Metadata,
                Thumbnails = new Dictionary<ThumbnailSize, string>(),
                UploadedBy = uploadedBy,
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
                DeletedOn = deleted ? result : DateTimeOffset.MinValue //only assign 'result' when it has been altered succesfully and deleted = true
            };
        }

        public static Post PostFromBlobProperties(string identifier, string blobUrl, BlobProperties props, PostClassification classification = PostClassification.Photo)
        {
            bool deleted = false;
            DateTimeOffset result = DateTimeOffset.MinValue;
            if (props.Metadata.Keys.Contains(BStorage.Meta.DeletedOn))
            {
                try
                {
                    deleted = DateTimeOffset.TryParse(props.Metadata[BStorage.Meta.DeletedOn], out result);
                }
                catch (Exception) { }  // swallowing this because it tells us deletedOn wasn't set so we can proceed as planned
            }

            // TODO: uploadedBy / userID will be implemented with the Auth milestone --> Done
            var uploadedBy = props.Metadata[BStorage.Meta.UploadedBy] ?? null;
            var oFilename = props.Metadata[BStorage.Meta.OriginalFilename] ?? identifier;
            var containerName = props.Metadata["container"] ?? BStorage.Containers.Uploads;
            var post = new Post();
            switch(classification) {
                case PostClassification.Photo: //TODO: enhance
                    var photo = ((Photo)post);
                    photo.Title = oFilename;
                    photo.SetCreatedOn();
                    photo.OriginalFilename = oFilename;
                    return photo;
                case PostClassification.Gallery: //TODO: enhance
                    var gallery = ((Gallery)post);
                    gallery.DisplayName = oFilename;
                    gallery.SetCreatedOn();
                    gallery.Description = String.Empty;
                    return gallery;
                default: break;
            }
            return post;
        }
        public static string Serialize<T>(T target) => JsonSerializer.Serialize((target), _options);
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
                T? dto = JsonSerializer.Deserialize<T>(blobMetadataJson, _options);
                return dto;
            }
            catch (Exception e) when (e is JsonException || e is  InvalidArgumentException || e is NotSupportedException)
            {
                throw new CCSerializationException($"Invalid argument '{nameof(blobMetadataJson)}' provided with value: '{blobMetadataJson}'.", e);
            }
        }
    }
}
