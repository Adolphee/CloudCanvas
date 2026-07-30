using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Posts.Photos.Commands.UploadFile;
using CloudCanvas.Application.Posts.Photos.Queries.GetAllPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotoByKey;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
using CloudCanvas.Application.Users.Commands.EnsureUserExists;

namespace CloudCanvas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public sealed class PhotosController(ISender sender, ILogger<PhotosController> logger) : ControllerBase
    {
        private readonly ISender _sender = sender;
        private readonly ILogger<PhotosController> _logger = logger;

        [HttpGet(Name = "GetAllPhotos")]
        public async Task<ActionResult<GetAllPhotosResult>> GetAsync(CancellationToken cancellation = default) 
            => Ok(await _sender.Send(new GetAllPhotosQuery(), cancellation));

        [HttpGet("single", Name = "GetPhotoById")]
        public async Task<ActionResult<PhotoDTO>> GetSingleByKeyAsync([FromQuery] string id, [FromQuery] string userId, CancellationToken cancellation = default)
        {
            _logger.LogInformation("Photo projection lookup with key: [id={PhotoId}, userId={userId}].", id, userId);
            var res = await _sender.Send(new GetPhotoByKeyQuery(new(id, userId)), cancellation);
            return res?.Photo != null? Ok(res.Photo): NotFound();
        }

        [HttpGet("user/{userId}", Name = "GetUserPhotos")]
        public async Task<ActionResult<GetUserPhotosResult>> GetUserPhotosAsync(string userId, CancellationToken cancellation = default) 
            => Ok(await _sender.Send(new GetUserPhotosQuery(userId), cancellation));

        [HttpPost(Name = "CreatePhoto")]
        public async Task<ActionResult<CreatePhotoResult>> CreatePhotoAsync([FromBody] CreatePhotoCommand command, CancellationToken cancellation = default) 
            => Ok(await _sender.Send(command, cancellation));

        [HttpPost("upload", Name = "UploadPhoto")]
        public async Task<ActionResult<CreatePhotoResult>> UploadPhotoAsync(IFormFile file, CancellationToken cancellation = default)
        {
            var appUser = (await _sender.Send(new EnsureUserExistsCommand(User.ToAppUser()), cancellation)).User;
            var uploadRes = await _sender.Send(new UploadFileCommand(file, appUser.Id), cancellation);
            var creationCommand = uploadRes.FileMetadata.ToPhoto(appUser.Id).IssueCreationCommand(appUser.ToCreator());
            var result = await _sender.Send(creationCommand, cancellation);
            return Ok(result);
        }
    }
}
