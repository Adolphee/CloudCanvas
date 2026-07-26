namespace CloudCanvas.Application.Users
{
    public record ApplicationUser
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string AboutMe { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string ProPicUrl { get; set; } = default!;
    }
}
