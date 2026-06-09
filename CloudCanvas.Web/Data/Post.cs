using CloudCanvas.Shared.Enums;
using CloudCanvas.Web.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace CloudCanvas.Web.Data
{
    public class Post: IPost, ICommentable
    {

        [Required]
        public Guid? Id { get; set; }
        public string? Url { get; set; } = default!;
        public DateTime? CreatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime? PublishedOn { get; set; }
        public DateTime? UnpublishedOn { get; set; }
        public long ContentLength { get; set; }
        [Required]
        public string? UserId { get; set; }
        [Required]
        public ApplicationUser? Author { get; set; }
        public List<Like> Likes { get; set; } = new();
        public List<Dislike> Dislikes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public bool CommentsEnabled { get; set; } = true;

        public bool Delete(ApplicationUser user, bool softDelete = true)
        {
            if (Author == null || Author.Id != user.Id || DeletedOn != DateTime.MinValue) return false;
            DeletedOn = DateTime.UtcNow;
            return true;
        }

        public Dislike? DisLike(ApplicationUser user)
        {
            if (Author == null || Author.Id == user.Id || Likes == null) return null; // obvious restriction
            if(Dislikes == null) Dislikes = new List<Dislike>();
            var disLike = new Dislike
            {
                PostId = Id, Post = this, User = user, UserId = user.Id,
            };
            Dislikes.Add(disLike);
            return disLike;
        }


        public bool IsDisLikedBy(ApplicationUser user)
        {
            return Dislikes != null /*&& Dislikes.Any(d => d.UserId == user.Id)*/;
        }

        public bool IsLikedBy(ApplicationUser user)
        {
            return Likes != null/* && Likes.Any(l => l.UserId == user.Id)*/;
        }

        public Like Like(ApplicationUser user)
        {
            var like = new Like{ UserId = user.Id, User = user, PostId = Id, Post = this };
            if (Likes == null) Likes = new List<Like>();
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


        public bool Publish(ApplicationUser user)
        {
            if(user == null || Author == null || Author.Id != user.Id || PublishedOn != DateTime.MinValue) return false;
            PublishedOn = DateTime.UtcNow;
            return true;
        }

        public bool RemoveDisLike(Dislike dislike)
        {
            if(IsDisLikedBy(dislike.User!))
            {
                Dislikes!.Remove(dislike);
                return true;
            }
            return false;
        }

        public bool ReportPost(ApplicationUser user, string reason)
        {
            throw new NotImplementedException();
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
    }
}