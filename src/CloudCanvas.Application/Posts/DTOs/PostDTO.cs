using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.DTOs
{
    public record PostDTO
    {
        [Required]
        public string? Id { get; init; }
        [Required]
        public string? UserId { get; init; }
        [Required]
        public Creator? Creator { get; init; } = default!;

        [Required]
        public string Classification { get; init; } = default!;
        public ReactionsOverviewDTO Reactions { get; init; } = new();
        public AuditableEntity TimeStamps { get; init; } = new();
    }
}
