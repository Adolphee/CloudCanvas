using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCanvas.Application.Posts.Comments.Commands.EditComment
{
    public sealed record EditCommentResponse
    {
        public required bool Success { get; init; }
        public required CommentDTO Comment { get; init; }
    }

    public sealed record EditCommentCommand: IRequest<EditCommentResponse>
    {
        public required string Id { get; init; }
        public required string Text { get; init; }
        public required string UserId { get; init; }
        public required string PostId { get; init; }
        public required string UserDisplayName { get; init; }
    }
}