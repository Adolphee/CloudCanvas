using Azure.Storage.Blobs.Models;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Infrastructure.DTOs
{
    /// <summary>
    /// Represents metadata and properties associated with a blob in a storage system.
    /// </summary>
    /// <remarks>This class provides detailed information about a blob, including its file name, URL,
    /// metadata,  content properties, and various operational states such as copy status, encryption details,  and
    /// access tier. It is designed to encapsulate all relevant data for managing and interacting  with blobs in a
    /// storage context.</remarks>
    public class BlobMetadata: FileMetadata
    {
        public DateTimeOffset DeletedOn { get; set; } = default!;
    }
}
