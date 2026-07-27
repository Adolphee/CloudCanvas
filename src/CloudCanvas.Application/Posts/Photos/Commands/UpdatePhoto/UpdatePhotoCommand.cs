using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Photos.Commands.UpdatePhoto
{
    public sealed record UpdatePhotoCommand: CreatePhotoCommand
    {
        [Required]
        public required string Id { get; set; }
    }
}
