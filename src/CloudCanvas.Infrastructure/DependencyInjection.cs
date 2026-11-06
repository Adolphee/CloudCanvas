using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Abstractions.Storage;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Interfaces;
using CloudCanvas.Application.Posts.Galleries.Interfaces;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Infrastructure.BlobStorage;
using CloudCanvas.Infrastructure.Common;
using CloudCanvas.Infrastructure.Projection;
using CloudCanvas.Infrastructure.Messaging;
using CloudCanvas.Infrastructure.Persistence;
using CloudCanvas.Infrastructure.Persistence.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudCanvas.Application.Posts.Comments;
using Microsoft.Extensions.Logging;
using CloudCanvas.Domain.Posts.Entities;
namespace CloudCanvas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {   // validate and Fail-Fast if any connection string is missing
            var connStrings = ConfigTool.GetConnectionStrings(config); 

            services.AddScoped<IMessenger, Messenger>();
            services.AddScoped<IMediaStorage, BlobStorageService>();
            services.AddScoped<IMessageBuilder, SBMessageBuilder>();
            services.AddScoped<IMessageFactory, MessageFactory>();
            services.AddScoped<IMessengerFactory, SBClientFactory>();

            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IPhotoProjectionStore, PhotoProjectionStore>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGalleryRepository, GalleryRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IGalleryProjectionStore, GalleryProjectionStore>();
            services.AddScoped<IPostRepository<Post>, PostRepositoryBase<Post>>();
            services.AddScoped<IPostRepository<Photo>, PhotoRepository>();
            services.AddScoped<IPostRepository<Gallery>, GalleryRepository>();
            services.AddTransient<IImageTool, ImageTool>();

            services.AddDbContext<CCDBContext>(options => options.UseSqlServer(config.GetConnectionString(SQLServer.ConnectionString), sql => sql.EnableRetryOnFailure()));
            services.AddSingleton(cc =>
            {
                var conn = connStrings[Application.Common.Constants.Projection.Uri];
                return new CosmosClient(conn, new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                        IgnoreNullValues = true
                    },
                    ConsistencyLevel = ConsistencyLevel.Eventual
                });
            });
            services.AddSingleton(cc => new BlobServiceClient(connStrings[BStorage.BSConnection]));
            services.AddSingleton(sp => new ServiceBusClient(connStrings[Secrets.FUMSGO]));
            return services;
        }
    }

}
