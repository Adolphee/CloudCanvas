using Azure.Storage.Blobs.Models;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Users;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Infrastructure.DTOs;
using CloudCanvas.Infrastructure.Identity;

namespace CloudCanvas.Infrastructure.Common.Extensions
{
    public static class InfraMapper
    {
        /// <summary>
        /// Converts the provided blob Properties and metadata into a <see cref="BlobMetadata"/> object.
        /// </summary>
        /// <remarks>This method maps the Properties of a blob, as represented by <see
        /// cref="BlobProperties"/>,  to a <see cref="BlobMetadata"/> object for further processing or use in the
        /// application.</remarks>
        /// <param name="identifier">The original name of the file before it was uploaded to the blob storage.</param>
        /// <param name="blobUrl">The URL of the blob in the storage system.</param>
        /// <param name="props">The Properties of the blob, including metadata, content details, and versioning information.</param>
        /// <returns>A <see cref="BlobMetadata"/> object containing metadata and Properties of the blob, such as its URL, 
        /// original file name, content details, and other relevant attributes.</returns>
        public static FileMetadata ToMetadata(this BlobProperties props, string identifier, string blobUrl)
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
            return new FileMetadata
            {
                Id = identifier,
                UserId = uploadedBy ?? "Unknown User",
                Location = blobUrl,
                OriginalFilename = oFilename,
                CreatedOn = props.CreatedOn,
                ContainerName = containerName,
                ProcessingStage = 0, // TOFIX: this is misleading when this method is not 
                Metadata = props.Metadata.ToDictionary(),
                Thumbnails = new Dictionary<ThumbnailSize, string>(),
                UploadedBy = uploadedBy,
                Name = identifier,
                Description = String.Empty, // future A.I. implementation will further process and fill in this description
                Tags = new List<string>(), // future A.I. implementation will further process and fill in these tags
                TagCount = props.TagCount,
                BlobType = props.BlobType.ToString(),
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
                CopyStatus = props.CopyStatus.ToString(),
                CopyProgress = props.CopyProgress,
                CopySourceUrl = props.CopySource?.ToString() ?? String.Empty,
                CopyCompletedOn = props.CopyCompletedOn,
                IsLatestVersion = props.IsLatestVersion,
                VersionId = props.VersionId,
                DeletedOn = deleted ? result : DateTimeOffset.MinValue //only assign 'result' when it has been altered succesfully and deleted = true
            };
        }


        public static User ToIdentityUser(this ApplicationUser user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName
        };

        public static ApplicationUser ToAppUser(this User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName
        };
    }
}
