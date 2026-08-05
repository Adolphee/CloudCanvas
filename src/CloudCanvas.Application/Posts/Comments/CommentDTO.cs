using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Comments
{
    public sealed record CommentDTO: PostDTO
    {
        [MaxLength(255), Required]
        public string Text { get; init; } = default!;
        public Creator Author { get; init; } = default!;
        public PostDTO TargetPost { get; init; } = default!;
    }
}
