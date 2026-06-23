using CloudCanvas.Domain.Addresses;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Reactions;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.User
{
    public interface IAppUser
    {
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AboutMe { get; set; }
        public string? ProPicUrl { get; set; }
        public Address? Address { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? BirthDay { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public DateTimeOffset? DeletedOn { get; set; }
        public string? Description { get; set; }
        public List<Reaction> Reactions { get; set; }

        public List<Post> Posts { get; set; }
        //public IAppUser(string userName, string firstName, string lastName, string emailAddress, DateTime birthDate);
        
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
}