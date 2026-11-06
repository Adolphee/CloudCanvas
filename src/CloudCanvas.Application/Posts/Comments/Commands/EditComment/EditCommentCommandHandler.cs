using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Comments.Commands.EditComment
{
    public sealed class EditCommentCommandHandler: IRequestHandler<EditCommentCommand, EditCommentResponse>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository<Post> _postRepository;
        private readonly ILogger<EditCommentCommandHandler> _logger;

        public EditCommentCommandHandler(ICommentRepository commentRepository, IPostRepository<Post> postRepository, ILogger<EditCommentCommandHandler> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<EditCommentResponse> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            
            var comment = await _commentRepository.SingleAsync(request.Id, cancellationToken);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID '{CommentId}' not found.", request.Id);
                throw new NotFoundException($"Comment with ID '{request.Id}' not found.");
            }

            comment.Text = request.Text;
            comment.SetModifiedOn();

            await _commentRepository.EditCommentAsync(comment, cancellationToken);

            return new EditCommentResponse
            {
                Success = true,
                Comment = comment.ToProjection()
            };
        }
    }
}