using CloudCanvas.Application.Users.Commands.EnsureUserExists;
using User = CloudCanvas.Application.Abstractions.Identity.ApplicationUser;

namespace CloudCanvas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [Authorize] // Only until I build a frontend UI flow for OAuth2.0 auth management
        [HttpGet("login")]
        public async Task<ActionResult<User?>> LoginAsync(CancellationToken cancellation = default)
        {
            var res = await _sender.Send(new EnsureUserExistsCommand(User.ToAppUser()), cancellation);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me() => Ok(User.ToAppUser()); // later we'll get the full user object from persistence

        [HttpPost("signOut")]
        public IActionResult SignOutUser() => NotFound(new
        {
            message = "Client should discard the bearer token to sign out."
        });
    }
}
