using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed record CreateThumbnailResult(string SrcContainer, ThumbnailSize Size, string ThumbnailUrl, PhotoDTO OriginalPhoto)
    {
    }
}