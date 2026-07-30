namespace CloudCanvas.Application.Abstractions.Identity
{
    public record ApplicationUser
    {
        public required string Id { get; init; } = default!;
        public required string Email { get; init; } = default!;
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string UserName { get; init; } = default!;
        public string AboutMe { get; init; } = default!;
        public string DisplayName { get; init; } = default!;
        public string ProPicUrl { get; init; } = default!;
    }
}
