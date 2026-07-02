using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Reactions;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.DTOs
{
    public record PostDTO
    {
        [Required]
        public string? Id { get; set; }
        [Required]
        public Creator? Creator { get; set; } = default!;

        [Required]
        public string Classification { get; set; } = default!;
        public ReactionsOverviewDTO Reactions { get; set; } = new();
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
    }
}
