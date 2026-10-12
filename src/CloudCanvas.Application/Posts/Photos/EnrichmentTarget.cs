namespace CloudCanvas.Application.Posts.Photos
{
    public record EnrichmentTarget(string id, string user_id, string url)
    {
        public List<string> tags {get; set; } = new();
        public string caption {get; set; } = default!;     
    }
}