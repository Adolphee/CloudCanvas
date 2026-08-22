using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Posts.Galleries
{
    public sealed record GalleryDTO: PostDTO
    {
        public List<GalleryItemDTO> Photos { get; set; } = new();
        public string DisplayName { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public List<string> UserTags { get; set; } = new();
        public bool CommentsEnabled { get; internal set; } = true;
        public int PhotosCount => Photos.Count;

        public GalleryDTO(string id, string displayName, CreatorMinimal user)
        {
            Id = id;
            Creator = user;
            DisplayName = displayName;
            Classification = PostClassification.Gallery.ToString();
        }
    }
}