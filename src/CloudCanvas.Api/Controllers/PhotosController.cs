using CloudCanvas.Application.Posts.Photos.Commands;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Contracts.IPost>;
namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class PhotosController(ICosmosRepo client) : ControllerBase
    {
        private readonly ICosmosRepo _client = client;

        [HttpGet(Name = "GetAllPhotos")]
        public async Task<GetAllPhotosQueryResult> GetAsync() => await new GetAllPhotosRequestHandler(_client).Handle(new GetAllPhotosQuery());

        [HttpGet("user/{userId}", Name = "GetUserPhotos")]
        public async Task<GetUserPhotosQueryResult> GetUserPhotosAsync(string userId) => await new GetUserPhotosRequestHandler(_client).Handle(new GetUserPhotosQuery(userId));

        [HttpPost(Name = "SavePhoto")]
        public async Task<SavePhotoQueryResult> CreatePhotoAsync([FromBody] SavePhotoCommand command) => await new SavePhotoRequestHandler(_client).Handle(command);
    }
}
