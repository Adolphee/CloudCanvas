using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Posts.Photos.Commands.UploadFile;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;
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
                cfg.RegisterServicesFromAssemblyContaining<GetAllPhotosQuery>();
                cfg.RegisterServicesFromAssemblyContaining<GetUserPhotosQuery>();
            });

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return services;
        }
    }
}
