using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public sealed record PhotoDTO: PostDTO
    {
        [Required]
        public string OriginalFilename { get; set; }
        [Required]
        public string Location { get; set; } = default!;
        public bool CommentsEnabled { get; set; } = true;
        public string? Description { get; set; } = default!;
        public string? Title { get; set; }
        public long ContentLength { get; set; }
        public List<string>? UserTags { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public PhotoDTO()
        {

        }
        public PhotoDTO(string id, Creator user, ReactionsOverviewDTO rOverview, string oFilename, string url)
        {
            Id = id;
            Creator = user;
            Reactions = rOverview;
            OriginalFilename = oFilename;
            Location = url;
        }
    }
}
