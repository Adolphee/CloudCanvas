using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Application.Posts.Galleries
{
    public sealed record GalleryDTO: PostDTO
    {
        public List<GalleryItemDTO> Photos { get; set; } = new();
        public string? DisplayName { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public List<string> UserTags { get; set; } = new();
        public bool CommentsEnabled { get; internal set; } = true;
        public int PhotosCount => Photos.Count;

        public GalleryDTO(string id, Creator user)
        {
            Id = id;
            Creator = user;
        }
    }
}