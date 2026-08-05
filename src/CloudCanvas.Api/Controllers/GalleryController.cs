using CloudCanvas.Application.Posts.Galleries;
using CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery;

namespace CloudCanvas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class GalleryController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        // GET: GalleryController
        public async Task<ActionResult<List<GalleryDTO>>> Index()
        {
            return Ok();
        }

        // POST: GalleryController/Create
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
