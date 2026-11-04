using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Posts.Photos
{
    public record EnrichmentTarget(string id, string user_id, string url)
    {
        public List<SmartTag> tags {get; set; } = new();
        public string caption {get; set; } = default!;     
    }
}