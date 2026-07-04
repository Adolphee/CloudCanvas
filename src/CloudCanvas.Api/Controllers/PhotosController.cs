using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Posts.Photos.Commands;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;
namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class PhotosController(ICosmosRepo client, IPhotoRepositoryEF ctx) : ControllerBase
    {
        private readonly ICosmosRepo _client = client;
        private readonly IPhotoRepositoryEF _context = ctx;

        [HttpGet(Name = "GetAllPhotos")]
        public async Task<GetAllPhotosQueryResult> GetAsync() => await new GetAllPhotosRequestHandler(_client).Handle(new GetAllPhotosQuery());

        [HttpGet("user/{userId}", Name = "GetUserPhotos")]
        public async Task<GetUserPhotosQueryResult> GetUserPhotosAsync(string userId) => await new GetUserPhotosRequestHandler(_client).Handle(new GetUserPhotosQuery(userId));

        [HttpPost(Name = "SavePhoto")]
        public async Task<SavePhotoQueryResult> CreatePhotoAsync([FromBody] SavePhotoCommand command, CancellationToken cancellation = default) 
            => await new SavePhotoRequestHandler(_client, _context).Handle(command, cancellation);
    }
}
