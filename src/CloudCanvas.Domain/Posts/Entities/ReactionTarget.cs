using System.ComponentModel.DataAnnotations.Schema;
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Reactions.Entities;

namespace CloudCanvas.Domain.Entities
{
    public abstract class ReactionTarget: AuditableEntity, IEntity, IUserOwned ,IReactionTarget
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public virtual List<Reaction> Reactions { get; init; } = [];
        
        #region REACTIONS

        [NotMapped]
        public virtual List<Like> Likes => [.. Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>()];
        
        [NotMapped]
        public virtual List<Dislike> Dislikes => [.. Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>()];
        
        [NotMapped]
        public virtual List<EmojiReaction> EmojiReactions => [.. Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>()];
        

        public virtual bool Dislike(string userId)
        {
            if (UserId == userId || Likes == null) return false; // obvious restriction
            var disLike = new Dislike
            {
                PostId = this.Id, UserId = userId
            };
            Dislikes.Add(disLike);
            return true;
        }

        public virtual bool IsDislikedBy(string user)
        {
            return Dislikes != null && Dislikes.Any(d => d.UserId == user);
        }

        public virtual bool IsLikedBy(string user)
        {
            return Likes != null && Likes.Any(l => l.UserId == user);
        }

        public virtual bool Like(string user)
        {
            if(String.IsNullOrWhiteSpace(user)) return false;
            var like = new Like { UserId = user, PostId = this.Id };
            Likes.Add(like);
            return true;
        }

        public virtual int DislikesCount()
        {
            if (Dislikes == null) return 0;
            return Dislikes.Count;
        }

        public virtual int LikesCount()
        {
            if (Likes == null) return 0;
            return Likes.Count;
        }

        public virtual int EmojiReactionsCount()
        {
            if (EmojiReactions == null) return 0;
            return EmojiReactions.Count;
        }

        public virtual bool UnLike(string user)
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

        public virtual bool RemoveDisLike(Dislike dislike)
        {
            if(IsDislikedBy(dislike.UserId!))
            {
                Dislikes!.Remove(dislike);
                return true;
            }
            return false;
        }
        #endregion

    }
}