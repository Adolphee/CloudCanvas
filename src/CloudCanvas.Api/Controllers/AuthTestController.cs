using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudCanvas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult Public()
    {
        return Ok(new { message = "This endpoint is public." });
    }

    [HttpGet("private")]
    [Authorize]
    public IActionResult Private()
    {
        return Ok(new
        {
            message = "You are authenticated.",
            isAuthenticated = User.Identity?.IsAuthenticated ?? false,
            name = User.Identity?.Name,
            oid = User.FindFirst("oid")?.Value,
            tid = User.FindFirst("tid")?.Value,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
