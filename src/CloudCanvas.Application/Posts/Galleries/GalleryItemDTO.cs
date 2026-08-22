using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Posts.Galleries
{
    public record GalleryItemDTO
    {
        public required string Location { get; init; }
        public required string Title { get; init; }
        public string? MediumThumbnail { get; init; }
        public required CreatorMinimal Creator { get; init; }
    }
}
