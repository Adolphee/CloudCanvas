using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using System.Text.Json;
using CloudCanvas.Shared.Constants;

namespace CloudCanvas.Shared.Services
{
    public class BlobMetaConverter : IBlobMetaConverter
    {
        public BlobMetaDTO FromBinaryData(BinaryData binary)
        {
            var dto = JsonSerializer.Deserialize<BlobMetaDTO>(binary);
            if (dto is null) throw new InvalidCastException($"{nameof(BlobMetaConverter)} could not parse {nameof(BinaryData)} to {nameof(BlobMetaDTO)}.");
            return dto;
        }

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
        public BlobMetaDTO FromBlobProperties(string originalFileName, string blobUrl, BlobProperties props)
        {
            BlobMetaDTO meta = new BlobMetaDTO();
            meta.UserId = Guid.NewGuid().ToString();
            meta.BlobUrl = blobUrl;
            meta.OriginalFileName = originalFileName;
            meta.CreatedOn = props.CreatedOn;
            meta.Metadata = props.Metadata;
            meta.ContentType = props.ContentType;
            meta.ContentLength = props.ContentLength;
            meta.CopyCompletedOn = props.CopyCompletedOn;
            meta.TagCount = props.TagCount;
            meta.ETag = props.ETag.ToString().Trim('\"');
            meta.SourceUrl = props.CopySource?.ToString();
            meta.ProcessingStage = ServiceBus.Subs.ExtractMetaData;
            meta.CreatedOn = props.CreatedOn;
            meta.ExpiresOn = props.ExpiresOn;
            meta.ContentLanguage = props.ContentLanguage;
            meta.ContentEncoding = props.ContentEncoding;
            meta.ContentDisposition = props.ContentDisposition;
            meta.ContentLength = props.ContentLength;
            meta.IsLatestVersion = props.IsLatestVersion;
            meta.VersionId = props.VersionId;
            meta.CopyId = props.CopyId;
            meta.LastAccessed = props.LastAccessed;
            meta.LastModified = props.LastModified;
            meta.IsLatestVersion = props.IsLatestVersion;
            meta.Tags = new List<string>(); // future A.I. implementation will further process and fill in these tags
            meta.BlobType = props.BlobType;
            meta.CopyProgress = props.CopyProgress;
            meta.CopyStatus = props.CopyStatus;
            return meta;
        }

        public string Serialize(ServiceBusMessageDTO ccMessageDto) => JsonSerializer.Serialize(ccMessageDto, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });
        public string Serialize(BlobMetaDTO meta) => JsonSerializer.Serialize(meta, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });
    }
}
