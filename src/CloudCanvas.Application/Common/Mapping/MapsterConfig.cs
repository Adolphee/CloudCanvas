using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Thumbnail;
using Mapster;

namespace CloudCanvas.Application.Common.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Photo, PhotoDTO>()
                .Map(dest => dest.Description, src => src.Caption)
                .Map(dest => dest.Classification, src => src.Classification.ToString())
                .Map(dest => dest.TimeStamps.CreatedOn, src => src.CreatedOn)
                .Map(dest => dest.TimeStamps.ModifiedOn, src => src.ModifiedOn)
                .Map(dest => dest.TimeStamps.DeletedOn, src => src.DeletedOn)
                .Map(dest => dest.Thumbnails,
                    src => (src.Thumbnails ?? new List<PhotoThumbnail>())
                        .ToDictionary(
                            t => t.Size.ToString(),
                            t => t.Url))
                .IgnoreNonMapped(true);



            config.NewConfig<FileMetadata, PhotoDTO>()
                .Map(dest => dest.Thumbnails,
                    src => (src.Thumbnails ?? new Dictionary<ThumbnailSize, string>())
                        .ToDictionary(
                            kvp => kvp.Key.ToString(),
                            kvp => kvp.Value))
                .IgnoreNonMapped(true);

            config.NewConfig<CreatePhotoCommand, Photo>()
                .Ignore(dest => dest.Thumbnails)
                .IgnoreNonMapped(true);

            config.NewConfig<FileMetadata, Photo>()
                .Map(dest => dest.Thumbnails,
                    src => (src.Thumbnails ?? new Dictionary<ThumbnailSize, string>())
                        .Select(kvp => new PhotoThumbnail
                        {
                            PhotoId = src.Id,
                            OriginalImageURL = src.Location,
                            Size = kvp.Key,
                            Url = kvp.Value
                        })
                        .ToList())
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.GalleryId)
                .Ignore(dest => dest.CreatedOn)
                .Ignore(dest => dest.ModifiedOn)
                .Ignore(dest => dest.DeletedOn)
                .IgnoreNonMapped(true);
        }
    }
}