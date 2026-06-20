using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Web.Entities;
    public enum PostCategory
    {
        Comment, Photo, Gallery
    }
    public class Post
    {

        #region PROPERTIES
        [Required]
        public string? Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        public string? Url { get; set; } = default!;
        private readonly static PostCategory PostCategory = PostCategory.Gallery;
        public long ContentLength { get; set; }
        public bool CommentsEnabled { get; set; } = true;
        public string? DisplayName { get; set; }

        public string? Title { get; set; }
        public string OriginalFilename { get; set; } = default!;
        public List<string> UserTags { get; set; } = new();

        public string? Description { get; set; }
        DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
        DateTimeOffset DeletedOn { get; set; }
        DateTimeOffset ModifiedOn { get; set; }
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }
        #endregion

        #region REACTIONS
        
        #endregion
    }