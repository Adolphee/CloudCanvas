using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

using User = CloudCanvas.Infrastructure.Identity.User;

namespace CloudCanvas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration config, CCDBContext context) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly CCDBContext _context = context;

        [Authorize]
        [HttpGet("login")]
        public async Task<IActionResult> Login(string? returnUrl = "/", CancellationToken cancellation = default)
        {
            var user = new User()
            {
                Id = User.FindFirstValue(CCClaimTypes.ObjectIdentfier)!,
                Email = User.FindFirstValue(ClaimTypes.Email),
                FirstName = User.FindFirstValue(ClaimTypes.GivenName),
                LastName = User.FindFirstValue(ClaimTypes.Surname),
                UserName = User.FindFirstValue(ClaimTypes.Email)
            };

            if (!await _context.Users.AnyAsync(u => u.Id == user.Id || u.Email == user.Email, cancellation))
            {
                await _context.Users.AddAsync(user, cancellation);
                await _context.SaveChangesAsync(cancellation);
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var user = User;

            var streetAddress = user.FindFirstValue(ClaimTypes.StreetAddress);
            var tenantId = user.FindFirstValue("tid");
            var subject = user.FindFirstValue("sub");
            var username = user.FindFirstValue(ClaimTypes.Email);
            var displayName = user.FindFirstValue("name");
            var email = user.FindFirstValue(ClaimTypes.Email);
            var identifier = user.FindFirstValue(CCClaimTypes.ObjectIdentfier);

            return Ok(new
            {
                identifier,
                displayName = user.FindFirstValue(CCClaimTypes.Name),
                username,
                firstName = user.FindFirstValue(ClaimTypes.GivenName),
                lastName = user.FindFirstValue(ClaimTypes.Surname),
                birthDay = user.FindFirstValue(ClaimTypes.DateOfBirth),
                email = user.FindFirstValue(ClaimTypes.Email),
                claims = user.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        [HttpPost("signOut")]
        public IActionResult SignOutUser()
        {
            return Ok(new
            {
                message = "Client should discard the bearer token to sign out."
            });
        }
    }
}
