using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Common.Mapping
{
    public static class CommentMappings
    {
        public static CommentDTO ToProjection(this Comment comment, string AuthorDisplayName = Unknown.DisplayName)
        {
            CommentDTO cmt = new()
            {
                Text = comment.Text,
                Creator = new CreatorMinimal(comment.UserId, AuthorDisplayName), 
                Id = comment.Id,
                UserId = comment.UserId,
                TimeStamps = new()
                {
                    CreatedOn = comment.CreatedOn
                }
            };

            cmt.TimeStamps.SetModifiedOn(comment.ModifiedOn);
            return cmt;
        }
    }
}