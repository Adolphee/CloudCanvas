using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Queries.GetPostById
{
    public sealed record GetPostByIdRequest(string PostId): IRequest<GetPostByIdResponse>;
    public sealed record GetPostByIdResponse(bool Exists = false, Post? Post = null, string? ErrorMessage = null);
    public sealed class GetPostByIdRequestHandler(IPostRepository<Post> postRepository, ILogger<GetPostByIdRequestHandler> logger): IRequestHandler<GetPostByIdRequest, GetPostByIdResponse>
    {
        private readonly IPostRepository<Post> _postRepository = postRepository;
        private readonly ILogger<GetPostByIdRequestHandler> _logger = logger;

        public async Task<GetPostByIdResponse> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.PostId, tracking: false, cancellation: cancellationToken);

            if (post == null)
            {
                return new GetPostByIdResponse
                {
                    Exists = false,
                    ErrorMessage = $"Post with ID '{request.PostId}' not found."
                };
            }

            return new GetPostByIdResponse
            {
                Exists = true,
                Post = post
            };
        }
    }
}