namespace CloudCanvas.Domain.Posts.Entities
{
    public class Gallery : Post
    {
        public List<Photo> Photos { get; set; } = [];
        public string? DisplayName { get; set; } = default!;
        public string? Description { get; set; } = default!;

        public Gallery(bool commentsEnabled = true)
        {
            CommentsEnabled = commentsEnabled;
        }

        public Gallery() { }
        // idea: ability restrict commenting to only users who have photos in the gallery,
        // or to only followers of the gallery owner, or favorites etc.
    }
}
