using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Posts.Photos.Commands.UploadFile
{
    public sealed record UploadFileResult
    {
        bool success => FileMetadata is not null;
        public FileMetadata FileMetadata { get; set; } = default!;

        public Photo ToPhoto(string userId) => FileMetadata?.ToPhoto(userId) ?? throw new ArgumentNullException(nameof(FileMetadata));
    }
}