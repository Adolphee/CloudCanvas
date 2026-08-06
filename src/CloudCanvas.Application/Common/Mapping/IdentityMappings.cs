using CloudCanvas.Application.Abstractions.Identity;
using System.Security.Claims;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class IdentityMappings
    {
        public static Creator ToCreator(this ApplicationUser user) => new(user.Id, user.UserName, $"{user.FirstName} {user.LastName}");
        public static ApplicationUser ToAppUser(this ClaimsPrincipal prinicipal) => new()
        {
            Id = prinicipal.FindFirstValue(CCClaimTypes.ObjectIdentfier)!,
            Email = prinicipal.FindFirstValue(ClaimTypes.Email)!,
            FirstName = prinicipal.FindFirstValue(ClaimTypes.GivenName)!,
            LastName = prinicipal.FindFirstValue(ClaimTypes.Surname)!,
            UserName = prinicipal.FindFirstValue(ClaimTypes.Email)!
        };
    }
}
