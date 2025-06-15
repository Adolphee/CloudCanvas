using Azure;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.DTOs
{
    public class BlobMetaDTO
    {
        public BlobProperties OriginalBlobProperties { get; set; }
        public string OriginalFileName { get; set; }
        public string BlobUrl { get; set; }
        public string ProcessingStage { get; set; }
        public string UploadedBy { get; set; }
        public string Project { get; set; }
        public string[] Tags { get; set; } // for future A.I. integration
        public string LastModified { get; set; }
        public string CreatedOn { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public string BlobType { get; set; }
        public string CopyCompletedOn { get; set; }
        public string CopyStatusDescription { get; set; }
        public string CopyId { get; set; }
        public string CopyProgress { get; set; }
        public string SourceUrl { get; set; }
        public int CopyStatus { get; set; }
        public int? BlobCopyStatus { get; set; }
        public bool IsIncrementalCopy { get; set; }
        public string DestinationSnapshot { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ETag { get; set; }
        public string ContentEncoding { get; set; }
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
        public string AccessTierChangedOn { get; set; }
        public string VersionId { get; set; }
        public bool IsLatestVersion { get; set; }
        public long TagCount { get; set; }
        public string ExpiresOn { get; set; }

        public bool IsSealed { get; set; }
        public string RehydratePriority { get; set; }

        public string LastAccessed { get; set; }

        public bool HasLegalHold { get; internal set; }

        public BlobMetaDTO()
        {
            Metadata = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);;
        }
    }
}
