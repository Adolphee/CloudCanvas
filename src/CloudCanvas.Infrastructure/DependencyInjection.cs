using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Abstractions.Storage;
using CloudCanvas.Application.Common;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Interfaces;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Infrastructure.BlobStorage;
using CloudCanvas.Infrastructure.Common;
using CloudCanvas.Infrastructure.Cosmos;
using CloudCanvas.Infrastructure.Messaging;
using CloudCanvas.Infrastructure.Persistence;
using CloudCanvas.Infrastructure.Persistence.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace CloudCanvas.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IPhotoRepository, PhotoRepositoryEF>();
            services.AddScoped<IMediaStorage, BlobStorageService>();
            services.AddScoped<IPhotoProjectionStore, PhotoProjectionStore>();
            services.AddScoped<IMessenger, Messenger>();
            services.AddScoped<IMessageBuilder, SBMessageBuilder>();
            services.AddScoped<IMessageFactory, MessageFactory>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddTransient<IImageTool, ImageTool>();

            services.AddDbContext<CCDBContext>(options => options.UseSqlServer(config.GetConnectionString(SQLServer.ConnectionString), sql => sql.EnableRetryOnFailure()));
            services.AddSingleton(cc =>
            {
                var endpoint = config.GetConnectionString(Projection.Uri);

                return new CosmosClient(endpoint, new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                        IgnoreNullValues = true
                    }
                });
            });
            services.AddSingleton(cc =>
            {
                var uri = config.GetConnectionString(BStorage.BSConnection);
                return new BlobServiceClient(uri);
            });
            services.AddSingleton<IMessengerFactory>(sp =>
            {
                var sendpoint = config.GetConnectionString(Secrets.FUMSGO);
                Validate.StringValue(nameof(sendpoint), sendpoint);     // Quick Validation even in startup, Fail-Fast principle
                var client = new ServiceBusClient(sendpoint);
                return new SBClientFactory(client);
            });
            return services;
        }
    }
}
