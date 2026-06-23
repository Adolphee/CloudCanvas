using CloudCanvas.Domain.Addresses;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Reactions;

namespace CloudCanvas.Domain.User
{
    public sealed record class AppUser : IAppUser
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
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
        public List<Reaction?> Reactions { get; set; } = new();
        public List<Post?> Posts { get; set; } = new();

        public AppUser() { }
        public AppUser(string userName, string firstName, string lastName, string emailAddress, DateTime birthDate)
        {

        }
    }
}
