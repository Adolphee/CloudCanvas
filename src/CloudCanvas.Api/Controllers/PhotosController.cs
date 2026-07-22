using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Posts.Photos.Commands.UploadFile;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;

namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class PhotosController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpGet(Name = "GetAllPhotos")]
        public async Task<ActionResult<GetAllPhotosResult>> GetAsync() => Ok(await _sender.Send(new GetAllPhotosQuery()));

        [HttpGet("user/{userId}", Name = "GetUserPhotos")]
        public async Task<ActionResult<GetUserPhotosResult>> GetUserPhotosAsync(string userId) => Ok(await _sender.Send(new GetUserPhotosQuery(userId)));

        [Authorize]
        [HttpPost(Name = "CreatePhoto")]
        public async Task<ActionResult<CreatePhotoResult>> CreatePhotoAsync([FromBody] CreatePhotoCommand command, CancellationToken cancellation = default) 
        {

            command.UserId = User.GetObjectId()!;
            var userName = User.FindFirstValue(ClaimTypes.Email);
            var displayName = User.FindFirstValue(CCClaimTypes.Name);
            command.Creator = new Creator(id: command.UserId, displayName: displayName, username: userName);
            var res = await _sender.Send(command, cancellation);

            return Ok(new { res.Success, res.Photo });
        }

        [Authorize]
        [HttpPost("upload", Name = "UploadPhoto")]
        public async Task<ActionResult<CreatePhotoResult>> UploadPhotoAsync(IFormFile file, CancellationToken cancellation = default)
        {
            var creator = new Creator(User.GetObjectId()!, User.FindFirstValue(ClaimTypes.Email), User.FindFirstValue(CCClaimTypes.Name));
            var uploadRes = await _sender.Send(new UploadFileCommand(file, creator.GetId()!), cancellation);
            
            var creationCommand = uploadRes.FileMetadata.ToPhoto(creator.GetId()!).IssueCreationCommand(creator);
            creationCommand.Creator = creator;
            var projection = await _sender.Send(creationCommand, cancellation);
            return Ok(new { projection.Success, projection.Photo });
        }
    }
}
