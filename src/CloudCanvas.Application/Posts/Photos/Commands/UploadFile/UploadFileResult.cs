using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts.Contracts;

namespace CloudCanvas.Application.Posts.Photos.Commands.UploadFile
{
    public sealed record UploadFileResult
    {
        public FileMetadata FileMetadata { get; set; }
    }
}