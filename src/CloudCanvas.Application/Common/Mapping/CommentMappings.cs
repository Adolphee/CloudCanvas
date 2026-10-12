using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class CommentMappings
    {
        public static CommentDTO ToProjection(this Comment comment, string AuthorDisplayName = default!) => new()
        {
            Text = comment.Text,
            Creator = new CreatorMinimal(comment.UserId, AuthorDisplayName), 
            Id = comment.Id, 
            Classification = "Comment",
            UserId = comment.UserId, 
            TimeStamps = new()
            {
                CreatedOn = comment.CreatedOn,
                DeletedOn = comment.DeletedOn,
                ModifiedOn = comment.ModifiedOn
            }, Reactions = new()
            {
                Likes = comment.LikesCount(),
                Dislikes = comment.DislikesCount(),
                EmojiReactions = comment.EmojiReactionsCount()
            }
        };
    }
}