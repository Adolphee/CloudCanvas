using Microsoft.AspNetCore.Identity;
namespace CloudCanvas.Web.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProPicUrl { get; set; }
    public Address? Address { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime DateOfBrith { get; set; }
    public DateTime LastModified { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? Description { get; set; }
}
