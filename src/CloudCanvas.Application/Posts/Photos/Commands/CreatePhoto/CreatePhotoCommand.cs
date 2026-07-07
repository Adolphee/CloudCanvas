using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.Common.Enums;
using System.Text.Json.Serialization;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto
{
    public record CreatePhotoCommand: TimeStampz, IRequest<CreatePhotoResult>
    {
        public string UserId { get; set; } = default!;
        public string? Caption { get; set; } = default!;
        public string OriginalFilename { get; set; } = default!;
        public string Location { get; set; } = default!;
        public string Title { get; set; } = default!;
        public Dictionary<string, string>? Thumbnails { get; set; } = new();
        public string? GalleryId { get; set; } = default!;
        public bool CommentsEnabled { get; set; } = true;
        public PostClassification Classification { get; set; } = PostClassification.Photo;
        public int ContentLength { get; set; }
        public List<string>? UserTags { get; set; } = new();
        public TimeStampz? TimeStamps { get; set; } = new();
        [JsonIgnore]
        public Creator? Creator = null;
    }
}
