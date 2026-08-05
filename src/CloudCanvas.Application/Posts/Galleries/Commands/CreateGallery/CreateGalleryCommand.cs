namespace CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery
{
    public sealed record CreateGalleryCommand: IRequest<CreateGalleryResult>
    {
        public string? Id { get; set; } // ID is determined by handler
        public required string UserId { get; init; }
        public required string DisplayName { get; init; } = default!;
        public required string Description { get; init; } = default!;
        public bool CommentsEnabled { get; init; } = true;

        public List<string> Photos { get; init; } = [];
        public Creator? Creator { get; init; }

        public CreateGalleryCommand NewWithCreator(Creator creator) => new()
        {
            Creator = creator,
            DisplayName = this.DisplayName,
            Description = this.Description,
            UserId = this.UserId,
            Id = this.Id,
            Photos = this.Photos,
            CommentsEnabled = this.CommentsEnabled
        };
    }
}
