using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Comments
{
    public sealed record CommentDTO
    {
        public required string Id { get; init; }
        public required string Text { get; init; }
        public required string Username { get; init; }
        public required string UserId { get; init; }
        public required DateTimeOffset CreatedOn { get; init; }
        public required DateTimeOffset ModifiedOn { get; init; }
    }
}
