using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts;

namespace CloudCanvas.Application.Posts.DTOs
{
    public sealed record GalleryDTO: PostDTO
    {
        public List<PhotoDTO> Photos { get; set; } = new();
        public string? DisplayName { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public List<string> UserTags { get; set; } = new();
        public GalleryDTO(string id, Creator user)
        {
            Id = id;
            Creator = user;
        }
    }
}