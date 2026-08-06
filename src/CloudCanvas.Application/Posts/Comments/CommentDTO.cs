using CloudCanvas.Application.Reactions.Common;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Comments
{
    public sealed record CommentDTO
    {
        [MaxLength(255), Required]
        public string Text { get; init; } = default!;
        public string AuthorDisplayName { get; init; } = default!;
        public DateTimeOffset CreatedOn { get; init; } = default!;
        public ReactionsOverviewDTO Reactions { get; init; } = new();
    }
}
