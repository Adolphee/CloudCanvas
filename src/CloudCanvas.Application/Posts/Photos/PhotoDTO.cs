using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Posts.Photos
{
    public sealed record PhotoDTO: PostDTO
    {
        public required string OriginalFilename { get; init; }
        public required string Location { get; init; } = default!;
        public long ContentLength { get; init; }
        public bool CommentsEnabled { get; init; } = true;
        public string? Description { get; init; } = default!;
        public string? SmartCaption { get; init; } = default!;
        public string? Title { get; init; }
        public List<string> UserTags { get; init; } = [];
        public List<string> SmartTags { get; init; } = [];
        public Dictionary<string, string> Thumbnails { get; init; } = [];
        public string? GalleryId { get; init; } = default!;

        public PhotoDTO()
        {
        }

        public PhotoDTO(string id, CreatorMinimal user, ReactionsOverviewDTO rOverview, string oFilename, string url)
        {
            Id = id;
            Creator = user;
            Reactions = rOverview;
            OriginalFilename = oFilename;
            Location = url;
            Classification = PostClassification.Photo.ToString();
        }
    }
}
