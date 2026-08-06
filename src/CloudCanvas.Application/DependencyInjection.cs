using CloudCanvas.Application.Posts.Comments.Commands.AddComment;
using CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Posts.Photos.Commands.UploadFile;
using CloudCanvas.Application.Posts.Photos.Queries.GetAllPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
using CloudCanvas.Application.Users.Commands.EnsureUserExists;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CloudCanvas.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<UploadFileCommand>();
                cfg.RegisterServicesFromAssemblyContaining<CreatePhotoCommand>();
                cfg.RegisterServicesFromAssemblyContaining<EnsureUserExistsCommand>();
                cfg.RegisterServicesFromAssemblyContaining<GetAllPhotosQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetUserPhotosQuery>();
                cfg.RegisterServicesFromAssemblyContaining<CreateGalleryCommand>();
                cfg.RegisterServicesFromAssemblyContaining<AddCommentCommand>();
            });

            services.Configure<JsonSerializerOptions>(options =>
            {   
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            return services;
        }
    }
}
