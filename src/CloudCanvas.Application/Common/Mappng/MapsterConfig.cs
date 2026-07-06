using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.SavePhoto;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Thumbnail;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudCanvas.Application.Common.Mappng
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
                .Map(dest => dest.Thumbnails, src => src.Thumbnails.Select(t => new Dictionary<string, string>() { { t.Size.ToString(), t.Url } }));

            config.NewConfig<CreatePhotoCommand, Photo>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.GalleryId)
                //.Ignore(dest => dest.CreatedOn) should be now()
                .IgnoreNonMapped(true);

            config.NewConfig<FileMetadata, PhotoDTO>()
                .Map(dest => dest.Thumbnails, src => src.Thumbnails.Select(t => new Dictionary<string, string>() { { t.Key.ToString(), t.Value } }));

            config.NewConfig<FileMetadata, Photo>()
                .Map(dest => dest.Thumbnails, src => src.Thumbnails.Select(t => new PhotoThumbnail
                    {
                        PhotoId = src.Id,
                        OriginalImageURL = src.Location,
                        Size = t.Key,
                        Url = t.Value
                    }).ToList())
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.GalleryId)
                .Ignore(dest => dest.CreatedOn)
                .Ignore(dest => dest.ModifiedOn)
                .Ignore(dest => dest.DeletedOn);
        }
    }
}
