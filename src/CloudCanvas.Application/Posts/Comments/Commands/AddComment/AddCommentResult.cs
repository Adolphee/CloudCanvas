namespace CloudCanvas.Application.Posts.Comments.Commands.AddComment
{
    public sealed record AddCommentResult
    {
        public CommentDTO Comment { get; init; } = default!;
    }
}