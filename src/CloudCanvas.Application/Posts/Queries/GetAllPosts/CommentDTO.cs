using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public sealed record CommentDTO: PostDTO
    {
        private readonly static PostClassification PostCategory = PostClassification.Comment;
        [MaxLength(255), Required]
        public string Text { get; set; } = default!;
        public Creator Author { get; set; } = default!;
        public PostDTO TargetPost { get; set; } = default!;
    }
}
