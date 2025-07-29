using Azure.Storage.Blobs.Models;

namespace CloudCanvas.Shared.DTOs
{
    /// <summary>
    /// Represents metadata and properties associated with a blob in a storage system.
    /// </summary>
    /// <remarks>This class provides detailed information about a blob, including its file name, URL,
    /// metadata,  content properties, and various operational states such as copy status, encryption details,  and
    /// access tier. It is designed to encapsulate all relevant data for managing and interacting  with blobs in a
    /// storage context.</remarks>
    public class BlobMetaDTO: GalleryItemDTO
    {
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

    }
}
