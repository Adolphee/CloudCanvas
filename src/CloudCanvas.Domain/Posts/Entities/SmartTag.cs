using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCanvas.Domain.Posts.Entities
{
    public sealed record SmartTag
    {
        public required string Id { get; init; } = default!;
        public required string Name { get; init; } = default!;
        public double Confidence { get; set; } = default!;
        public string? PostId { get; set; } = default!;
    }
}