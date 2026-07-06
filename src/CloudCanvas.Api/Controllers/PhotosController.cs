using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.SavePhoto;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
using CloudCanvas.Infrastructure.Identity;
using CloudCanvas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;
namespace CloudCanvas.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class PhotosController(ICosmosRepo client, IPhotoRepositoryEF ctx, CCDBContext context) : ControllerBase
    {
        private readonly ICosmosRepo _client = client;
        private readonly CCDBContext _context = context;
        private readonly IPhotoRepositoryEF _photos = ctx;

        [HttpGet(Name = "GetAllPhotos")]
        public async Task<IActionResult> GetAsync()
        {
            var photos = await new GetAllPhotosRequestHandler(_client).Handle(new GetAllPhotosQuery());
            return Ok(new { photos.Count, photos });
        }

        [HttpGet("user/{userId}", Name = "GetUserPhotos")]
        public async Task<IActionResult> GetUserPhotosAsync(string userId)
        {
            var photos = await new GetUserPhotosRequestHandler(_client).Handle(new GetUserPhotosQuery(userId));
            return Ok(new { photos.Count, photos });
        }

        [Authorize]
        [HttpPost(Name = "SavePhoto")]
        public async Task<PhotoDTO> CreatePhotoAsync([FromBody] CreatePhotoCommand command, CancellationToken cancellation = default) {
            command.UserId = User.GetObjectId()!;
             var user = new User()
            {
                Id = User.FindFirstValue(CCClaimTypes.ObjectIdentfier)!,
                Email = User.FindFirstValue(ClaimTypes.Email),
                FirstName = User.FindFirstValue(ClaimTypes.GivenName),
                LastName = User.FindFirstValue(ClaimTypes.Surname),
                UserName = User.FindFirstValue(ClaimTypes.Email),
                DisplayName = User.FindFirstValue(CCClaimTypes.Name)
             };

            if (!await _context.Users.AnyAsync(u => u.Id == user.Id || u.Email == user.Email, cancellation))
            {
                await _context.Users.AddAsync(user, cancellation);
                await _context.SaveChangesAsync(cancellation);
            }
            command.Creator = new Creator(id : command.UserId, displayName: user.DisplayName, username: user.Email);
            return await new CreatePhotoCommandHandler(_client, _photos).Handle(command, cancellation);
        }
    }
}
