using CloudCanvas.Application.Posts.Comments.Commands.AddComment;

namespace CloudCanvas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public sealed class CommentsController(ISender sender): ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpPost]
        public async Task<ActionResult> AddComment([FromBody] AddCommentCommand command, CancellationToken cancellation)
        {
            var creator = User.ToAppUser().ToCreator();
            return await _sender.Send(command.NewWithCreator(creator)) is AddCommentResult result
                ? Ok(result)
                : BadRequest();
        }
    }
}
