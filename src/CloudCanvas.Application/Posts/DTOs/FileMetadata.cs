using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.DTOs
{
    /// <summary>
    /// Represents metadata and Properties associated with a blob in a storage system.
    /// </summary>
    /// <remarks>This class provides detailed information about a blob, including its file name, URL,
    /// metadata,  content Properties, and various operational states such as copy status, encryption details,  and
    /// access tier. It is designed to encapsulate all relevant data for managing and interacting  with blobs in a
    /// storage context.</remarks>
    public record FileMetadata: EssentialFileInfo
    {
        [Required, MaxLength(100)]
        public string Name { get; init; } = default!;
        [Required, Range(0, 4)]
        public int ProcessingStage { get; init; }
        public string? ContentEncoding { get; init; }
        public string? ContainerName { get; init; }
        public string? BlobType { get; init; }
        public List<string> Tags { get; init; } = new(); // for future A.I. integration for auto-tagging
        public string? ContentType { get; init; }
        public long TagCount { get; init; }
        public string? ContentLanguage { get; init; }
        public bool HasLegalHold { get; internal init; }
        public Dictionary<string, string> Metadata { get; init; } = new(); // any additional/custom metadata
        public DateTimeOffset CopyCompletedOn { get; init; } = new();
        public string? CopyStatusDescription { get; init; }
        public string? CopyId { get; init; }
        public string? CopyProgress { get; init; }
        public string? CopySourceUrl { get; init; }
        public string? CopyStatus { get; init; }
        public bool IsIncrementalCopy { get; init; }
        public string? DestinationSnapshot { get; init; }
        public string? ETag { get; init; }
        public string? ContentDisposition { get; init; }
        public string? CacheControl { get; init; }
        public long BlobSequenceNumber { get; init; }
        public string? AcceptRanges { get; init; }
        public int BlobCommittedBlockCount { get; init; }
        public bool IsServerEncrypted { get; init; }
        public string? EncryptionKeySha256 { get; init; }
        public string? EncryptionScope { get; init; }
        public string? AccessTier { get; init; }
        public bool AccessTierInferred { get; init; }
        public string? ArchiveStatus { get; init; }
        public DateTimeOffset AccessTierChangedOn { get; init; }
        public string? VersionId { get; init; }
        public bool IsLatestVersion { get; init; }
        public DateTimeOffset ExpiresOn { get; init; }
        public bool IsSealed { get; init; }
        public DateTimeOffset LastAccessed { get; init; }
        public DateTimeOffset DeletedOn { get; init; } = default!;
}
}
