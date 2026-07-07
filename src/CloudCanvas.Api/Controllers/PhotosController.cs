using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
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
        public async Task<ActionResult<CreatePhotoResult>> CreatePhotoAsync([FromBody] CreatePhotoCommand command, CancellationToken cancellation = default) {

            command.UserId = User.GetObjectId()!;
            var userName = User.FindFirstValue(ClaimTypes.Email);
            var displayName = User.FindFirstValue(CCClaimTypes.Name);
            command.Creator = new Creator(id: command.UserId, displayName: displayName, username: userName);
            return Ok(await _sender.Send(command, cancellation));
        }
    }
}
