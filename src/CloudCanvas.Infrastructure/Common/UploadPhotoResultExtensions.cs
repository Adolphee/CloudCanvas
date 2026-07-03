using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;
using Mapster;

namespace CloudCanvas.Infrastructure.Common
{
    public static class UploadPhotoResultExtensions
    {
        public static BlobMetadata FromPhotoResult(this FileMetadata result) => result.Adapt<BlobMetadata>();
    }
}
