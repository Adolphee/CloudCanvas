using Azure.Storage.Blobs.Models;
using CloudCanvas.Functions.DTOs;

namespace CloudCanvas.Functions.Interfaces
{
    /// <summary>
    /// Defines methods for converting between blob metadata and their representations.
    /// </summary>
    /// <remarks>This interface provides functionality to create a <see cref="BlobMetaDTO"/> from blob-related
    /// data  and to serialize a <see cref="CloudCanvasMessageDTO"/> to a string representation.</remarks>
    public interface IBlobMetaConverter
    {
        public BlobMetaDTO ToBlobMeta(string originalFileName, string blobUrl, BlobProperties blobProps);
        public string ToString(CloudCanvasMessageDTO blobMetaDTO);
    }
}
