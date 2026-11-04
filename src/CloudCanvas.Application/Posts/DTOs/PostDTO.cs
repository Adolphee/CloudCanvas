using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Abstractions;

namespace CloudCanvas.Application.Posts.DTOs
{
    public record PostDTO
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required CreatorMinimal Creator { get; init; }
        public ReactionsOverviewDTO Reactions { get; init; } = new();
        public required TimeStamps TimeStamps { get; init; }
    }
}
