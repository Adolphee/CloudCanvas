using Azure.Storage.Blobs.Models;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Thumbnail;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.DTOs
{
    /// <summary>
    /// Represents metadata and properties associated with a blob in a storage system.
    /// </summary>
    /// <remarks>This class provides detailed information about a blob, including its file name, URL,
    /// metadata,  content properties, and various operational states such as copy status, encryption details,  and
    /// access tier. It is designed to encapsulate all relevant data for managing and interacting  with blobs in a
    /// storage context.</remarks>
    public class FileMetadata: GalleryItemDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;
        [Required, Range(0, 4)]
        public int ProcessingStage { get; set; }
        public string? ContentEncoding { get; set; }
        public string? ContainerName { get; set; }
        public BlobType BlobType { get; set; }
        public List<string> Tags { get; set; } = new(); // for future A.I. integration for auto-tagging
        public string? ContentType { get; set; }
        public long TagCount { get; set; }
        public string? ContentLanguage { get; set; }
        public bool HasLegalHold { get; internal set; }
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>(); // any additional/custom metadata
        public DateTimeOffset CopyCompletedOn { get; set; } = new();
        public string? CopyStatusDescription { get; set; }
        public string? CopyId { get; set; }
        public string? CopyProgress { get; set; }
        public string? CopySourceUrl { get; set; }
        public CopyStatus? CopyStatus { get; set; }
        public bool IsIncrementalCopy { get; set; }
        public string? DestinationSnapshot { get; set; }
        public string? ETag { get; set; }
        public string? ContentDisposition { get; set; }
        public string? CacheControl { get; set; }
        public long BlobSequenceNumber { get; set; }
        public string? AcceptRanges { get; set; }
        public int BlobCommittedBlockCount { get; set; }
        public bool IsServerEncrypted { get; set; }
        public string? EncryptionKeySha256 { get; set; }
        public string? EncryptionScope { get; set; }
        public string? AccessTier { get; set; }
        public bool AccessTierInferred { get; set; }
        public string? ArchiveStatus { get; set; }
        public DateTimeOffset AccessTierChangedOn { get; set; }
        public string? VersionId { get; set; }
        public bool IsLatestVersion { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public bool IsSealed { get; set; }
        public RehydratePriority RehydratePriority { get; set; }
        public DateTimeOffset LastAccessed { get; set; }

        public Photo ToPhoto(PostClassification type = PostClassification.Photo)
        {
            var thumb = this.Thumbnails;
            return new Photo
            {
                Id = Id,
                OriginalFilename = OriginalFilename,
                Title = OriginalFilename,
                Caption = Description,
                UserTags = UserTags ?? default!,
                ContentLength = ContentLength,
                Location = this.Location,
                UserId = UserId!,
                Classification = PostClassification.Photo,
                Thumbnails = thumb.Select(t => new PhotoThumbnail
                {
                    PhotoId = Id,
                    OriginalImageURL = Location,
                    Size = t.Key,
                    Url = t.Value
                }).ToList(),
                CreatedOn = this.CreatedOn,
                ModifiedOn = this.LastModified,
                CommentsEnabled = this.CommentsEnabled ?? false
            };
        }

        public PhotoDTO ToPhotoDTO(PostClassification type = PostClassification.Photo)
        {
            var thumb = this.Thumbnails;
            return new PhotoDTO
            {
                Id = Id,
                OriginalFilename = OriginalFilename,
                Title = OriginalFilename,
                Description = Description,
                UserTags = UserTags ?? default!,
                ContentLength = ContentLength,
                Location = this.Location,
                Creator = new Creator
                {
                    Id = this.UserId!,
                    DisplayName = "Anonymous User", ///TODO: make creator info dynamic
                    UserName = "anon12345"
                },
                Classification = nameof(PostClassification.Photo),
                Thumbnails = thumb,
                CreatedOn = this.CreatedOn,
                ModifiedOn = this.LastModified,
                CommentsEnabled = this.CommentsEnabled ?? false
            };
        }
    }
}
