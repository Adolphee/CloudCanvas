using System.Diagnostics.CodeAnalysis;

namespace CloudCanvas.Application.Posts.DTOs
{
    public sealed record Creator: CreatorMinimal
    {
        public required string UserName { get; init; }

        [SetsRequiredMembers]
        public Creator(string id, string username, string displayName): base(id, displayName)
        {
            Id = id;
            UserName = username ?? Unknown.Username;
            DisplayName = displayName ?? Unknown.DisplayName;
        }

        public string GetId() => Id;
        public string GetUserName() => UserName;

        public CreatorMinimal MinimalVersion() => new(Id, DisplayName);
    }
}