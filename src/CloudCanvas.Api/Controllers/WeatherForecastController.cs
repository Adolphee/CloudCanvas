using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Queries.GetAllPosts;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Infrastructure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Handler = CloudCanvas.Application.Posts.Queries.GetAllPosts.GetAllPostsRequestHandler;
using ICosmos = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Post>;
namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Handler _handler;
        private readonly IPostsRepository<Post> _client;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICosmos _cosmos;
        public WeatherForecastController(CosmosClientWrapper<Post> client, ICosmos cosmos)
        {
            _client = client;
            _handler = new Handler(_client, cosmos);
            _cosmos = cosmos;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<GetAllPhotosQueryResult> Get()
        {
            var result = await _handler.Handle(new GetAllPostsQuery
            {
                UserId = Guid.NewGuid().ToString(),
                ContainerName = CloudCosmos.Containers.BlobMeta
            });
            return result;
        }
    }
}
