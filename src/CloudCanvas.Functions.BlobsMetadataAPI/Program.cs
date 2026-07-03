using Azure.Identity;
using Azure.Storage.Blobs;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure.BlobStorage;
using CloudCanvas.Infrastructure.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddScoped<GetAllPhotosRequestHandler>();
builder.Services.AddScoped<IPostsRepository<IPost>, CosmosClientWrapper<IPost>>();
builder.Services.AddSingleton(cc =>
{
    var accountEndpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);

    var credential = new DefaultAzureCredential();
    return new CosmosClient(accountEndpoint, credential, new CosmosClientOptions
    {
        ConnectionMode = ConnectionMode.Gateway,
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });
});

builder.Services.AddScoped<BlobStorageService>();
builder.Services.AddSingleton(bc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BStorage.Uri);
    var credential = new DefaultAzureCredential();
    return new BlobServiceClient(new Uri(endpoint!), credential);
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
