using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;

namespace CloudCanvas.Application.Posts.Photos.Commands.UploadFile
{
    public sealed record UploadFileResult
    {
        bool success => FileMetadata is not null;
        public FileMetadata FileMetadata { get; set; } = default!;

        public Photo ToPhoto(string userId) => FileMetadata?.ToPhoto(userId) ?? throw new ArgumentNullException(nameof(FileMetadata));
    }
}