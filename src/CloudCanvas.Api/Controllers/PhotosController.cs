using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Queries.GetAllPosts;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Handler = CloudCanvas.Application.Posts.Queries.GetAllPosts.GetAllPostsRequestHandler;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Contracts.IPost>;
namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class PhotosController(ICosmosRepo client) : ControllerBase
    {
        private readonly Handler _handler = new Handler(client);

        [HttpGet(Name = "GetAllPosts")]
        public async Task<GetAllPhotosQueryResult> Get() => await _handler.Handle(new GetAllPostsQuery());
    }
}
