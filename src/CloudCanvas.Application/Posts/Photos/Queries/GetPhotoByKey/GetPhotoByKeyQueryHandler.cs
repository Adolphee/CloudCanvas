using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotoByKey
{
    public class GetPhotoByKeyQueryHandler(IPhotoProjectionStore store, ILogger<GetPhotoByKeyQueryHandler> logger) : IRequestHandler<GetPhotoByKeyQuery, GetPhotoByKeyResult>
    {
        private readonly IPhotoProjectionStore _projection = store;
        private readonly ILogger<GetPhotoByKeyQueryHandler> _logger = logger;
        public async Task<GetPhotoByKeyResult> Handle(GetPhotoByKeyQuery req, CancellationToken cancellationToken = default)
        {
            try
            {
                return new GetPhotoByKeyResult(await _projection.SingleAsync(req.LookupKey, cancellationToken));
            }
            catch (ProjectionNotFoundException e)
            {
                _logger.LogWarning(e, "Photo projection not found. [id={PhotoId}, userId={userId}].", req.LookupKey.Id, req.LookupKey.UserId);
                return new GetPhotoByKeyResult(null);
            }
        }
    }
}
