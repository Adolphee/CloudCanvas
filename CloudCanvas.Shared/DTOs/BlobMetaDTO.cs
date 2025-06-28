using Azure.Storage.Blobs.Models;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Shared.DTOs
{
    /// <summary>
    /// Represents metadata and properties associated with a blob in a storage system.
    /// </summary>
    /// <remarks>This class provides detailed information about a blob, including its file name, URL,
    /// metadata,  content properties, and various operational states such as copy status, encryption details,  and
    /// access tier. It is designed to encapsulate all relevant data for managing and interacting  with blobs in a
    /// storage context.</remarks>
    public class BlobMetaDTO: MetadataDocumentBase
    {
        public string OriginalFileName { get; set; }
        public string BlobUrl { get; set; }
        public string ProcessingStage { get; set; }
        public string UploadedBy { get; set; }
        public string Project { get; set; }
        public List<string> Tags { get; set; } // for future A.I. integration for auto-tagging
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public IDictionary<string, string> Metadata { get; set; } // any additional/custom metadata
        public BlobType BlobType { get; set; }
        public DateTimeOffset CopyCompletedOn { get; set; }
        public string CopyStatusDescription { get; set; }
        public string CopyId { get; set; }
        public string CopyProgress { get; set; }
        public string SourceUrl { get; set; }
        public CopyStatus CopyStatus { get; set; }
        public bool IsIncrementalCopy { get; set; }
        public string DestinationSnapshot { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }
        public string? ContentEncoding { get; set; }
        public string ContentDisposition { get; set; }
        public string ContentLanguage { get; set; }
        public string CacheControl { get; set; }
        public long BlobSequenceNumber { get; set; }
        public string AcceptRanges { get; set; }
        public int BlobCommittedBlockCount { get; set; }
        public bool IsServerEncrypted { get; set; }
        public string EncryptionKeySha256 { get; set; }
        public string EncryptionScope { get; set; }
        public string AccessTier { get; set; }
        public bool AccessTierInferred { get; set; }
        public string ArchiveStatus { get; set; }
        public DateTimeOffset AccessTierChangedOn { get; set; }
        public string VersionId { get; set; }
        public bool IsLatestVersion { get; set; }
        public long TagCount { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public bool IsSealed { get; set; }
        public RehydratePriority RehydratePriority { get; set; }

        public DateTimeOffset LastAccessed { get; set; }

        public bool HasLegalHold { get; internal set; }

    }
}
