using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Reactions;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public record PostDTO
    {
        [Required]
        public string? Id { get; set; }
        [Required]
        public Creator? Creator { get; set; } = default!;

        [Required]
        private static string Classification { get; set; }
        public ReactionsOverviewDTO Reactions { get; set; } = new();
        public DateTimeOffset CreatedOn { get; set; }
    }
}
