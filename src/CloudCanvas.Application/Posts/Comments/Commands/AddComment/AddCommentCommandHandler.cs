using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Comments.Commands.AddComment
{
    public sealed class AddCommentCommandHandler(ICommentRepository commentRepo, ILogger<AddCommentCommandHandler> logger) : IRequestHandler<AddCommentCommand, AddCommentResult>
    {
        private readonly ICommentRepository _commentRepo = commentRepo;
        private readonly ILogger<AddCommentCommandHandler> _logger = logger;
        public async Task<AddCommentResult> Handle(AddCommentCommand cmd, CancellationToken cancellation = default)
        {
            if (cmd.Creator == null) throw new ArgumentNullException(nameof(cmd.Creator));
            var cmt = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Text = cmd.Text,
                UserId = cmd.UserId,
                PostId = cmd.PostId,
                UserDisplayName = cmd.Creator.DisplayName
            };
            var res = await _commentRepo.AddCommentAsync(cmt, cancellation);
            // future feature: Raise event message, trigger notification, etc.
            return new AddCommentResult { Comment = cmt.ToProjection(cmd.Creator.DisplayName!) };
        }
    }
}