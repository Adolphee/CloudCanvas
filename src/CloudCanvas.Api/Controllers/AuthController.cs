using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Users;
using CloudCanvas.Application.Users.Commands.EnsureUserExists;
using CloudCanvas.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

using User = CloudCanvas.Infrastructure.Identity.User;

namespace CloudCanvas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ISender sender, IUserRepository repo, IConfiguration config) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly IUserRepository _repo = repo;
        private readonly ISender _sender = sender;

        [Authorize]
        [HttpGet("login")]
        public async Task<ActionResult<ApplicationUser?>> Login(string? returnUrl = "/", CancellationToken cancellation = default)
        {
            var user = GetApplicationUser();
            var res = await _sender.Send(new EnsureUserExistsCommand(user), cancellation);
            return Ok(res);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var user = GetApplicationUser();
            return Ok(user);
        }

        [HttpPost("signOut")]
        public IActionResult SignOutUser()
        {
            return Ok(new
            {
                message = "Client should discard the bearer token to sign out."
            });
        }

        private ApplicationUser GetApplicationUser() => new ApplicationUser()
        {
            Id = User.FindFirstValue(CCClaimTypes.ObjectIdentfier)!,
            Email = User.FindFirstValue(ClaimTypes.Email)!,
            FirstName = User.FindFirstValue(ClaimTypes.GivenName)!,
            LastName = User.FindFirstValue(ClaimTypes.Surname)!,
            UserName = User.FindFirstValue(ClaimTypes.Email)!,
        };
    }
}
