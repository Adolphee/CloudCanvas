using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace CloudCanvas.Web.Data;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; } = default!;
    public string? LastName { get; set; } = default!;
    public string? DisplayName { get; set; } = default!;
    public string? ProPicUrl { get; set; } = default!;
    public Address? Address { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? DateOfBrith { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string? Description { get; set; } = default!; 
    public List<Reaction> Reactions { get; set; } = new();
    public List<Post> Posts { get; set; } = new();

    #region NOT MAPPED PROPERTIES
    [NotMapped]
    public List<Comment> Comments => Posts.OfType<Comment>().ToList();
    [NotMapped]
    public List<Photo> Photos => Posts.OfType<Photo>().ToList();
    [NotMapped]
    public List<Gallery> Galleries => Posts.OfType<Gallery>().ToList();
    [NotMapped]
    public List<Like> Likes => Reactions.Where(r => r.Type == ReactionType.Like)
        .OfType<Like>().ToList();
    [NotMapped]
    public List<Dislike> Dislikes => Reactions.Where(r => r.Type.Equals(ReactionType.Dislike))
        .OfType<Dislike>().ToList();
    #endregion
}
