using CloudCanvas.Domain.Addresses;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Infrastructure.Identity;

public class User : IdentityUser
{
    public string? FirstName { get; set; } = default!;
    public string? LastName { get; set; } = default!;
    public string? AboutMe { get; set; } = default!;
    public string? ProPicUrl { get; set; } = default!;
    public Address? Address { get; set; }
    public DateTimeOffset? CreatedOn { get; set; }
    public DateTimeOffset? BirthDay { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? Description { get; set; } = default!; 
    public List<Reaction> Reactions { get; set; } = new();
    public List<Post> Posts { get; set; } = new();
    public User()
    {
        AboutMe = UserName;
    }
    public List<Comment> Comments { get; set; }

    #region NOT MAPPED PROPERTIES

    [NotMapped]
    public List<Photo> Photos => Posts.OfType<Photo>().ToList();
    
    public List<Gallery> Galleries => Posts.OfType<Gallery>().ToList();
    [NotMapped]
    public List<Like> Likes => Reactions.Where(r => r.Type == ReactionType.Like)
        .OfType<Like>().ToList();
    [NotMapped]
    public List<Dislike> Dislikes => Reactions.Where(r => r.Type.Equals(ReactionType.Dislike))
        .OfType<Dislike>().ToList();
    #endregion
}
