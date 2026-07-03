
using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Domain.Posts
{
    public record class Gallery : Post
    {
        private readonly static PostClassification PostCategory = PostClassification.Gallery;
        public List<Photo> Photos { get; set; } = new();
        public string? DisplayName { get; set; } = default!;
        public string? Description { get; set; } = default!;
        //public List<string>? UserTags { get; set; } = new();

        public Gallery(bool commentsEnabled = true)
        {
            CommentsEnabled = commentsEnabled;
        }

        public Gallery() { }
        // idea: ability restrict commenting to only users who have photos in the gallery,
        // or to only followers of the gallery owner, or favorites etc.
    }
}
