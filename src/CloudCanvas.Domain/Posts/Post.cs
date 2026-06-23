using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts
{
    public class Post: TimeStamped, IPost, ICommentable
    {

        #region PROPERTIES
        [Required]
        public PostClassification Classification { get; set; } = PostClassification.Photo;
        [Required]
        public string? Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        public string? Url { get; set; } = default!;
        public long ContentLength { get; set; }
        public bool CommentsEnabled { get; set; } = true;
        [Required]
        public AppUser? Author { get; set; } = default!;

        internal List<Reaction> Reactions = new();

        

        [NotMapped]
        public List<Like> Likes => Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>().ToList();
        
        [NotMapped]
        public List<Dislike> Dislikes => Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>().ToList();
        
        [NotMapped]
        public List<EmojiReaction> EmojiReactions => Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>().ToList();

        [NotMapped]
        public List<Comment> Comments { get; set; } = new();

        DateTimeOffset IHasTimestamps.CreatedOn { get; set; }
        DateTimeOffset IHasTimestamps.DeletedOn { get; set; }
        DateTimeOffset IHasTimestamps.ModifiedOn { get; set; }
        public DateTimeOffset PublishedOn { get; set; }
        public DateTimeOffset UnpublishedOn { get; set; }

        List<Reaction> IPost.Reactions { get; set; } = new();
        public List<string> UserTags { get; set; } = new();
        //public string? Title { get; set; } = default!;
        //public string? OriginalFilename { get; set; } = default!;
        #endregion

        #region REACTIONS
        public bool Delete(AppUser user, bool softDelete = true)
        {
            if (Author == null || Author.Id != user.Id || DeletedOn != DateTime.MinValue) return false;
            DeletedOn = DateTime.UtcNow;
            return true;
        }

        public Dislike? Dislike(AppUser user)
        {
            if (Author == null || Author.Id == user.Id || Likes == null) return null; // obvious restriction
            var disLike = new Dislike
            {
                PostId = this.Id, User = user
            };
            Dislikes.Add(disLike);
            return disLike;
        }

        public bool IsDislikedBy(AppUser user)
        {
            return Dislikes != null && Dislikes.Any(d => d.User.Id == user.Id);
        }

        public bool IsLikedBy(AppUser user)
        {
            return Likes != null && Likes.Any(l => l.User.Id == user.Id);
        }

        public Like Like(AppUser user)
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

        public bool UnLike(AppUser user)
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
        public bool UnPublish(AppUser user)
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

        public bool Publish(AppUser user)
        {
            if(user == null || Author == null || Author.Id != user.Id || PublishedOn != DateTime.MinValue) return false;
            PublishedOn = DateTime.UtcNow;
            return true;
        }

        public bool ReportPost(AppUser user, string reason)
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