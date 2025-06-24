using Azure.Storage.Blobs.Models;
using CloudCanvas.Constants;
using CloudCanvas.Functions.DTOs;
using CloudCanvas.Functions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudCanvas.Functions.Services
{
    public class BlobMetaConverter : IBlobMetaConverter
    {
        public BlobMetaDTO ToBlobMeta(string blobUrl, BlobProperties props)
        {
            BlobMetaDTO meta = new BlobMetaDTO();
            meta.BlobUrl = blobUrl;
            meta.CreatedOn = props.CreatedOn.ToUniversalTime().ToString();
            meta.Metadata = props.Metadata;
            meta.ContentType = props.ContentType;
            meta.ContentLength = props.ContentLength;
            meta.CopyCompletedOn = props.CopyCompletedOn.ToUniversalTime().ToString();
            meta.TagCount = props.TagCount;
            meta.ETag = props.ETag.ToString().Trim('\"');
            meta.SourceUrl = props.CopySource?.ToString();
            meta.ProcessingStage = ServiceBus.Subs.ExtractMetaData;
            meta.CreatedOn = props.CreatedOn.ToUniversalTime().ToString();
            meta.ExpiresOn = props.ExpiresOn.ToUniversalTime().ToString();
            meta.ContentLanguage = props.ContentLanguage;
            meta.LastAccessed = props.LastAccessed.ToUniversalTime().ToString();
            meta.LastModified = props.LastModified.ToUniversalTime().ToString();
            meta.IsLatestVersion = props.IsLatestVersion;
            meta.Tags = []; // future A.I. implementation will further process and fill in these tags
            meta.BlobType = props.BlobType.ToString();
            return meta;
        }

        public string ToString(CloudCanvasMessageDTO blobMetaDTO) => JsonSerializer.Serialize(blobMetaDTO);
    }
}
