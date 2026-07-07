using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CloudCanvas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
            => services.AddScoped<IPhotoRepository, PhotoRepositoryEF>();
    }
}
