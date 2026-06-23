using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public sealed record Creator
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public Creator() { }
        public Creator(string id, string? username, string? displayName)
        {
            Id = id;
            UserName = username?? "Unknown User";
            DisplayName = displayName?? "No display name" ;
        }
    }


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