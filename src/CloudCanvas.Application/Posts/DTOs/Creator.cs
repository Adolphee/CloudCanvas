namespace CloudCanvas.Application.Posts.DTOs
{
    public sealed record Creator
    {
        public string? Id { get; init; }
        public string? UserName { get; init; }
        public string? DisplayName { get; init; }
        public Creator() { }
        public Creator(string id, string? username = null, string? displayName = null)
        {
            Id = id;
            UserName = username ?? "Unknown User";
            DisplayName = displayName ?? "No display name";
        }

        public string? GetId() => Id;
        public string? GetUserName() => UserName;
        private Creator ResetId() => new Creator(default, UserName, DisplayName);
        private void ResetUserName() => new Creator(Id, default, DisplayName);

        public Creator SetDisplayNameOnly(string? displayName) => new Creator(Id, UserName, displayName);
    }
}
