using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Utilities;
using CloudCanvas.Web.Interfaces;

namespace CloudCanvas.Web.Data
{
    public class Gallery : Post
    {
        public List<Photo>? Photos { get; set; } = new();
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public List<string>? UserTags { get; set; } = new();

        public Gallery(bool commentsEnabled = true)
        {
            CommentsEnabled = commentsEnabled;
        }

        public Gallery() { }
        // idea: ability restrict commenting to only users who have photos in the gallery,
        // or to only followers of the gallery owner, or favorites etc.
    }
}
