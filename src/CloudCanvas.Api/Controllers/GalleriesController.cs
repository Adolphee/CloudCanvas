using CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery;

namespace CloudCanvas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public sealed class GalleriesController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        // POST: GalleriesController/Create
        [HttpPost(Name = "CreateGallery")]
        public async Task<ActionResult> Create([FromBody] CreateGalleryCommand command)
        {
            var completeCommand = command.NewWithCreator(User.ToAppUser().ToCreator());
            return await _sender.Send(completeCommand) is CreateGalleryResult result
                ? Ok(result)
                : BadRequest();
        }
    }
}
