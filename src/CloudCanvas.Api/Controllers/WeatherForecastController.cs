using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Queries.GetAllPosts;
using CloudCanvas.Domain.Posts.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Handler = CloudCanvas.Application.Posts.Queries.GetAllPosts.GetAllPostsRequestHandler<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Handler _handler;
        private readonly IPostsRepository<IPost> _client;
        //private readonly GraphServiceClient _graphServiceClient;
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        public WeatherForecastController(IPostsRepository<IPost> client)
        {
            _client = client;
            _handler = new Handler(_client);
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<List<IPost>> Get()
        {
            var result = await _handler.Handle(new GetAllPostsQuery
            {
                UserId = Guid.NewGuid().ToString(),
                ContainerName = CloudCosmos.Containers.BlobMeta
            });
            return result.Posts;
        } // 
    }
}
