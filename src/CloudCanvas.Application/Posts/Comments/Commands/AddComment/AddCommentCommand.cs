using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Comments.Commands.AddComment
{
    public sealed record AddCommentCommand: IRequest<AddCommentResult>
    {
        [Required, MaxLength(255)]
        public required string Text { get; init; }
        [Required, MaxLength(50)]
        public required string PostId { get; init; }
        [Required, MaxLength(50)]
        public required string UserId { get; init; }
        public Creator? Creator { get; init; }

        public AddCommentCommand NewWithCreator(Creator creator) => this with
        {
            Creator = creator,
            PostId = this.PostId,
            UserId = creator.Id,
            Text = this.Text
        };
    }
}