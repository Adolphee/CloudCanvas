namespace CloudCanvas.Application.Reactions.Common
{
    public sealed record ReactionDTO
    {
        public int Count { get; init; }
        public required string Url { get; init; }
    }
}
