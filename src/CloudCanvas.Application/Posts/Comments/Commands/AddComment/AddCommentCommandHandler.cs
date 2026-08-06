using CloudCanvas.Domain.Posts.Entities;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Comments.Commands.AddComment
{
    public sealed class AddCommentCommandHandler(ICommentRepository commentRepo, ILogger<AddCommentCommandHandler> logger) : IRequestHandler<AddCommentCommand, AddCommentResult>
    {
        public readonly ICommentRepository _commentRepo = commentRepo;
        public readonly ILogger<AddCommentCommandHandler> _logger = logger;
        public async Task<AddCommentResult> Handle(AddCommentCommand cmd, CancellationToken cancellation = default)
        {
            if (cmd.Creator == null) throw new ArgumentNullException(nameof(cmd.Creator));
            var cmt = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                Text = cmd.Text,
                PostId = cmd.PostId,
                UserId = cmd.UserId,
                UserName = cmd.Creator.UserName!
            };
            var res = await _commentRepo.AddCommentAsync(cmt, cancellation);
            // Raise event message
            return new AddCommentResult { Comment = cmt.ToProjection(cmd.Creator.DisplayName!) };
        }
    }
}
