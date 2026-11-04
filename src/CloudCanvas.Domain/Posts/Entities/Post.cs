using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Entities;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Reactions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts.Entities
{
    public class Post: ReactionTarget, IPost, ICommentable
    {
        #region PROPERTIES
        public string Location { get; set; } = default!;
        public List<Comment> Comments { get; init; } = [];
        public long ContentLength { get; set; }
        public bool CommentsEnabled { get; set; } = true;
        public DateTimeOffset PublishedOn { get; protected set; }
        public DateTimeOffset UnpublishedOn { get; protected set; }
        public List<string> UserTags { get; set; } = [];
        public bool IsPublished => PublishedOn != DateTimeOffset.MinValue && UnpublishedOn == DateTimeOffset.MinValue;
        public bool IsUnpublished => UnpublishedOn != DateTimeOffset.MinValue;
        public bool IsDeleted => DeletedOn != DateTimeOffset.MinValue;
        #endregion

        #region REACTIONS
        
        public virtual bool AddComment(Comment comment)
        {
            if (!CommentsEnabled) return false;
            if (comment == null || Comments == null || !Comments.Any(c => c.Id == comment.Id)) return false;
            Comments.Add(comment);
            return true;
        }

        public virtual bool RemoveComment(Comment comment)
        {
            if (!CommentsEnabled) return false;
            if (comment == null || Comments == null) return false;
            Comments.Remove(comment);
            return true;
        }

        public bool Delete(string user, bool softDelete = true)
        {
            if (UserId == null || UserId != user || DeletedOn != DateTime.MinValue) return false;
            DeletedOn = DateTime.UtcNow;
            return true;
        }
        #endregion
        
        #region PUBLICATION
        public bool UnPublish(string user)
        {
            if(UserId == null || UserId != user || PublishedOn == DateTime.MinValue || UnpublishedOn != DateTime.MinValue || Likes == null) return false;
            var like = Likes.FirstOrDefault(l => l.UserId == user);
            if (like != null)
            {
                Likes.Remove(like);
                UnpublishedOn = DateTime.UtcNow;
                return true;
            }
            return false;
        }

        public bool Publish(string user)
        {
            if(user == null || UserId == null || UserId != user || PublishedOn != DateTime.MinValue) return false;
            PublishedOn = DateTime.UtcNow;
            return true;
        }

        public bool ReportPost(string user, string reason)
        {
            throw new NotImplementedException();
        }

        public bool SetPublishedOn()
        {
            PublishedOn = DateTime.UtcNow;
            return true;
        }

        public bool SetUnpublishedOn()
        {
            UnpublishedOn = DateTimeOffset.UtcNow;
            return true;
        }
        #endregion
    }
}