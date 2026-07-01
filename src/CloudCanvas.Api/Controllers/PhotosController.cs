using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Queries.GetAllPosts;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure.Cosmos;
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

        [HttpGet(Name = "GetAllPosts")]
        public async Task<GetAllPhotosQueryResult> Get() => await new GetAllPostsRequestHandler(_client).Handle(new GetAllPostsQuery());

        [HttpGet(Name = "GetUserPosts")]
        public async Task<GetUserPhotosQueryResult> GetUserPhotos(string usr) => await new GetUserPhotosRequestHandler(_client).Handle(new GetUserPhotosQuery(usr));


    }
}
