using CloudCanvas.Application.Reactions.Common;

namespace CloudCanvas.Application.Posts.DTOs
{
    public sealed record PhotoDTO: PostDTO
    {
        public required string OriginalFilename { get; init; }
        public required string Location { get; init; } = default!;
        public bool CommentsEnabled { get; init; } = true;
        public string? Description { get; init; } = default!;
        public string? Title { get; init; }
        public long ContentLength { get; init; }
        public List<string> UserTags { get; init; } = [];
        public Dictionary<string, string> Thumbnails { get; init; } = [];
        public string? GalleryId { get; init; } = default!;

        public PhotoDTO()
        {

        }

        public PhotoDTO(string id, Creator user, ReactionsOverviewDTO rOverview, string oFilename, string url)
        {
            Id = id;
            Creator = user;
            Reactions = rOverview;
            OriginalFilename = oFilename;
            Location = url;
        }
    }
}
