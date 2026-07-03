using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Reactions.Common
{
    public sealed record ReactionsOverviewDTO
    {
        public int Interactions => Likes + Dislikes + EmojiReactions;
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int EmojiReactions { get; set; }
    }
}
