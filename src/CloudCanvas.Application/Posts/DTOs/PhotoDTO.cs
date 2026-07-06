using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Thumbnail;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.DTOs
{
    public sealed record PhotoDTO: PostDTO
    {
        [Required]
        public string OriginalFilename { get; set; }
        [Required]
        public string Location { get; set; } = default!;
        public bool CommentsEnabled { get; set; } = true;
        public string? Description { get; set; } = default!;
        public string? Title { get; set; }
        public long ContentLength { get; set; }
        public List<string>? UserTags { get; set; }
        public Dictionary<string, string> Thumbnails { get; set; } = new();
        public string? GalleryId { get; set; } = default!;

        public PhotoDTO()
        {

        }
        public PhotoDTO(string id, Creator user, ReactionsOverviewDTO rOverview, string oFilename, string url)
        {
            Id = id;
            Creator = user;
            Reactions = rOverview;
            OriginalFilename = oFilename;
            Location = url;
        }
    }
}
