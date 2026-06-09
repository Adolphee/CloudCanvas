using Microsoft.AspNetCore.Identity;
namespace CloudCanvas.Web.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string? ProPicUrl { get; set; } = default!;
    public Address? Address { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? DateOfBrith { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? Description { get; set; } = default!; 
    public List<Reaction>? Reactions { get; set; } = new();
    public List<Comment>? Comments { get; set; } = new();
    public List<Photo>? Photos { get; set; } = new();
    public List<Gallery>? Galleries { get; set; } = new();
}
