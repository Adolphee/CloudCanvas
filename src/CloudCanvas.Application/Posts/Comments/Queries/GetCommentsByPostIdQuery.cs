using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Comments.Queries
{
    public sealed record GetCommentsByPostIdQuery(string PostId): IRequest<List<CommentDTO>>;
    public sealed record GetCommentsByPostIdResponse(List<CommentDTO> Comments): IRequest<List<CommentDTO>>;

    public sealed class GetCommentsByPostIdQueryHandler(IPostRepository<Post> postRepository, ICommentRepository commentRepository, ILogger<GetCommentsByPostIdQueryHandler> logger): IRequestHandler<GetCommentsByPostIdQuery, List<CommentDTO>>
    {
        private readonly IPostRepository<Post> _postRepository = postRepository;
        private readonly ICommentRepository _commentRepository = commentRepository;
        private readonly ILogger<GetCommentsByPostIdQueryHandler> _logger = logger;

        public async Task<List<CommentDTO>> Handle(GetCommentsByPostIdQuery request, CancellationToken cancellationToken)
        {
            var postExists = await _postRepository.ExistsAsync(request.PostId, cancellationToken);
            if (!postExists)
            {
                _logger.LogWarning("Post with ID '{PostId}' not found.", request.PostId);
                throw new PostNotFoundException($"Post with ID '{request.PostId}' not found.");
            }
            var comments = await _commentRepository.GetAllCommentsAsync(request.PostId, cancellationToken);
            return comments.Select(c => c.ToProjection()).ToList();
        }
    }
}