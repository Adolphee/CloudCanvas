using Azure.Storage.Blobs.Models;
using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Infrastructure.BlobStorage
{
    /// <summary>
    /// Defines methods for converting between blob metadata and their representations.
    /// </summary>
    /// <remarks>This interface provides functionality to create a <see cref="BlobMetadata"/> from blob-related
    /// data  and to serialize a <see cref="ServiceBusMessageDTO"/> to a string representation.</remarks>
    public interface IBlobMetadataSerializer
    {
        BlobMetadata FromBinaryData(BinaryData binary);
        BlobMetadata FromBlobProperties(string originalFileName, string blobUrl, BlobProperties blobProps);
        string Serialize(BlobMetadata blobMetaDTO);
    }
}
