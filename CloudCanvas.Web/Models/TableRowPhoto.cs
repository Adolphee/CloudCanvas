namespace CloudCanvas.Web.Models
{
    public class TableRowPhoto
    {
        public string Id { get; set; } = default!;
        public string Url { get; set; } = default!;
        public string ThumbnailUrl { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTimeOffset LastModified { get; set; } = default!;
        public DateTimeOffset CreatedOn { get; set; } = default!;
        public string GalleryUrl { get; set; } = default!;
        public string GalleryName { get; set; } = default!;
        public string AuthorDisplayName { get; set; } = default!;
        public string AuthorId { get; set; } = default!;
        public long ContentLength { get; set; }
        public string ContainerName { get; set; } = default!;

    }
}
