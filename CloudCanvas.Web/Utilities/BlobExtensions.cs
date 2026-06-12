using CloudCanvas.Shared.DTOs;
using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Utilities
{
    public static class BlobExtensions
    {
        public static Photo Convert(this GalleryItemDTO dto, ApplicationUser user)
        {
            var photo = new Photo
            {
                Id = dto.Id,
                UserId = dto.UploadedBy,
                Author = user,
                ContentLength = dto.ContentLength,
                CreatedOn = dto.CreatedOn,
                ModifiedOn = dto.LastModified,
                DeletedOn = dto.DeletedOn ?? DateTimeOffset.MinValue,
                Url = dto.Url,
                OriginalFilename = dto.OriginalFilename,
                Title = dto.DisplayName
            };
            foreach (var item in dto.Thumbnails)
            {
                photo.Thumbnails.Add(new PhotoThumbnail
                {
                    Size = item.Key,
                    Url = item.Value,
                    PostId = photo.Id,
                    OriginalImageURL = photo.Url,
                    OriginalPhoto = photo
                });
            }
            return photo;
        }
    }
}
