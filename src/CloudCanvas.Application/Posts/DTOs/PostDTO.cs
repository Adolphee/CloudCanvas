using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.DTOs
{
    public record PostDTO
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required CreatorMinimal Creator { get; init; } = default!;
        public required string Classification { get; init; } = default!;
        public ReactionsOverviewDTO Reactions { get; init; } = new();
        public AuditableEntity TimeStamps { get; init; } = new();
    }
}
