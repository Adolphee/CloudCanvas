using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Reactions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts.Entities
{
    public abstract class Post: AuditableEntity, IPost, ICommentable
    {
        #region PROPERTIES
        public required string Id { get; set; }
        public required string UserId { get; set; } = default!;
        public string Location { get; set; } = default!;
        public long ContentLength { get; set; }
        public bool CommentsEnabled { get; set; } = true;
        public List<Reaction> Reactions { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }

        public List<string> UserTags { get; set; } = [];
        #endregion

        #region REACTIONS

        [NotMapped]
        public List<Like> Likes => [.. Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>()];
        
        [NotMapped]
        public List<Dislike> Dislikes => [.. Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>()];
        
        [NotMapped]
        public List<EmojiReaction> EmojiReactions => [.. Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>()];
        
        public bool Delete(string user, bool softDelete = true)
        {
            if (UserId == null || UserId != user || DeletedOn != DateTime.MinValue) return false;
            DeletedOn = DateTime.UtcNow;
            return true;
        }

        public bool Dislike(string userId)
        {
            if (UserId == userId || Likes == null) return false; // obvious restriction
            var disLike = new Dislike
            {
                PostId = this.Id, UserId = userId
            };
            Dislikes.Add(disLike);
            return true;
        }

        public bool IsDislikedBy(string user)
        {
            return Dislikes != null && Dislikes.Any(d => d.UserId == user);
        }

        public bool IsLikedBy(string user)
        {
            return Likes != null && Likes.Any(l => l.UserId == user);
        }

        public bool Like(string user)
        {
            if(String.IsNullOrWhiteSpace(user)) return false;
            var like = new Like { UserId = user, PostId = this.Id };
            Likes.Add(like);
            return true;
        }

        public int DislikesCount()
        {
            if (Dislikes == null) return 0;
            return Dislikes.Count;
        }

        public int LikesCount()
        {
            if (Likes == null) return 0;
            return Likes.Count;
        }

        public int EmojiReactionsCount()
        {
            if (EmojiReactions == null) return 0;
            return EmojiReactions.Count;
        }

        public bool UnLike(string user)
        {
            if (Likes == null || !IsLikedBy(user)) return false;
            var like = Likes.FirstOrDefault(l => l.UserId == user);
            if (like != null)
            {
                Likes.Remove(like);
                return true;
            }
            return false;
        }

        public bool RemoveDisLike(Dislike dislike)
        {
            if(IsDislikedBy(dislike.UserId!))
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