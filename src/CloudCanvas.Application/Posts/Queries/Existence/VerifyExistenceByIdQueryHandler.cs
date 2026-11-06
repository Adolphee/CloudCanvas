using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Queries.Existence
{
    public sealed record VerifyExistenceByIdQuery(string postId): IRequest<bool>;
    public sealed class VerifyExistenceByIdQueryHandler (IPostRepository<Post> postRepository, ILogger<VerifyExistenceByIdQueryHandler> logger) : IRequestHandler<VerifyExistenceByIdQuery, bool>
    {
        public readonly IPostRepository<Post> _postRepository = postRepository;
        public readonly ILogger<VerifyExistenceByIdQueryHandler> _logger = logger;
        public Task<bool> Handle(VerifyExistenceByIdQuery request, CancellationToken cancellationToken)
        {
            Validate.StringValue(nameof(request.postId), request.postId);
            return _postRepository.ExistsAsync(request.postId, cancellationToken);
        }
    }
}