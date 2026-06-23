using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Reactions.Common
{
    public sealed record ReactionDTO
    {
        public int Count { get; set; }
        public string Url { get; set; }
    }
}
