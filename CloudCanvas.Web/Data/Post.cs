using CloudCanvas.Shared.Enums;
using CloudCanvas.Web.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace CloudCanvas.Web.Data
{
    public abstract class Post: TimeStamped, IPost, ICommentable
    {

        #region PROPERTIES
        [Required]
        public string? Id { get; set; }
        public string? Url { get; set; } = default!;
        public long ContentLength { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public ApplicationUser? Author { get; set; }

        public List<Reaction> Reactions = new();
        [NotMapped]
        public List<Like> Likes => Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>().ToList();
        [NotMapped]
        public List<Dislike> Dislikes => Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>().ToList();
        [NotMapped]
        public List<EmojiReaction> EmojiReactions => Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>().ToList();
        public List<Comment> Comments { get; set; } = new();
        public bool CommentsEnabled { get; set; } = true;
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }
        #endregion

        #region REACTIONS
        public bool Delete(ApplicationUser user, bool softDelete = true)
        {
            if (Author == null || Author.Id != user.Id || DeletedOn != DateTime.MinValue) return false;
            DeletedOn = DateTime.UtcNow;
            return true;
        }

        public Dislike? Dislike(ApplicationUser user)
        {
            if (Author == null || Author.Id == user.Id || Likes == null) return null; // obvious restriction
            var disLike = new Dislike
            {
                PostId = this.Id, User = user
            };
            Dislikes.Add(disLike);
            return disLike;
        }

        public bool IsDislikedBy(ApplicationUser user)
        {
            return Dislikes != null && Dislikes.Any(d => d.User.Id == user.Id);
        }

        public bool IsLikedBy(ApplicationUser user)
        {
            return Likes != null && Likes.Any(l => l.User.Id == user.Id);
        }

        public Like Like(ApplicationUser user)
        {
            var like = new Like{ User = user, PostId = this.Id };
            Likes.Add(like);
            return like;
        }

        public int DisLikesCount()
        {
            if (Dislikes == null) return 0;
            return Dislikes.Count;
        }

        public int LikesCount()
        {
            if (Likes == null) return 0;
            return Likes.Count;
        }

        public bool UnLike(ApplicationUser user)
        {
            if (Likes == null || !IsLikedBy(user)) return false;
            var like = Likes.FirstOrDefault(l => l.User == user);
            if (like != null)
            {
                Likes.Remove(like);
                return true;
            }
            return false;
        }

        public bool RemoveDisLike(Dislike dislike)
        {
            if(IsDislikedBy(dislike.User!))
            {
                Dislikes!.Remove(dislike);
                return true;
            }
            return false;
        }
        public bool AddComment(Comment comment)
        {
            if (!CommentsEnabled) return false;
            if (comment == null || Comments == null || !Comments.Any(c => c.Id == comment.Id)) return false;
            Comments.Add(comment);
            return true;
        }
        public bool RemoveComment(Comment comment)
        {
            if (!CommentsEnabled) return false;
            if (comment == null || Comments == null) return false;
            Comments.Remove(comment);
            return true;
        }

        #endregion

        #region PUBLICATION
        public bool UnPublish(ApplicationUser user)
        {
            if(Author == null || Author.Id != user.Id || PublishedOn == DateTime.MinValue || UnpublishedOn != DateTime.MinValue || Likes == null) return false;
            var like = Likes.FirstOrDefault(l => l.User == user);
            if (like != null)
            {
                Likes.Remove(like);
                UnpublishedOn = DateTime.UtcNow;
                return true;
            }
            return false;
        }

        public bool Publish(ApplicationUser user)
        {
            if(user == null || Author == null || Author.Id != user.Id || PublishedOn != DateTime.MinValue) return false;
            PublishedOn = DateTime.UtcNow;
            return true;
        }

        public bool ReportPost(ApplicationUser user, string reason)
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