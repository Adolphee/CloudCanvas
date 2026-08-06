namespace CloudCanvas.Application.Posts.Comments.Commands.AddComment
{
    public sealed record AddCommentCommand: IRequest<AddCommentResult>
    {
        public required string Text { get; init; }
        public required string PostId { get; init; }
        public required string UserId { get; init; }
        public Creator? Creator { get; init; }

        public AddCommentCommand NewWithCreator(Creator creator) => this with
        {
            Creator = creator
        };
    }
}
