using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed record CreateThumbnailResult(string SrcContainer, ThumbnailSize Size, string ThumbnailUrl, PhotoDTO OriginalPhoto)
    {
    }
}