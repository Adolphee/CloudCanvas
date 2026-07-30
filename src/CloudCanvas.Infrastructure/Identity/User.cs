using CloudCanvas.Domain.Addresses;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Reactions.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Infrastructure.Identity;

public class User : IdentityUser
{
    public string? FirstName { get; set; } = default!;
    public string? LastName { get; set; } = default!;
    public string? AboutMe { get; set; } = default!;
    public string DisplayName { get; set; }
    public string? ProPicUrl { get; set; } = default!;
    public Address? Address { get; set; }
    public DateTimeOffset? CreatedOn { get; set; }
    public DateTimeOffset? BirthDay { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? Description { get; set; } = default!; 
    public List<Reaction> Reactions { get; set; } = [];
    public List<Post> Posts { get; set; } = [];
    public User()
    {
        DisplayName = UserName!;
    }
    public List<Comment> Comments { get; set; } = [];

    #region NOT MAPPED PROPERTIES

    [NotMapped]
    public List<Photo> Photos => [.. Posts.OfType<Photo>()];

    public List<Gallery> Galleries => [.. Posts.OfType<Gallery>()];
    [NotMapped]
    public List<Like> Likes => [.. Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>()];
    [NotMapped]
    public List<Dislike> Dislikes => [.. Reactions.Where(r => r.Type.Equals(ReactionType.Dislike)).OfType<Dislike>()];

    #endregion
}
