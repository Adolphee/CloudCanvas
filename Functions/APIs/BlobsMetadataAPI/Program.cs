using Azure.Identity;
using Azure.Storage.Blobs;
using CloudCanvas.Shared;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddTransient<CosmosClientWrapper>();
builder.Services.AddSingleton(cc =>
{
    var accountEndpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);
    Validate.StringValue(nameof(accountEndpoint), accountEndpoint);

    var credential = new DefaultAzureCredential();
    var cosmosClient = new CosmosClient(accountEndpoint, credential, new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    }); ;
    return cosmosClient;
});

builder.Services.AddTransient<BlobStorageService>();
builder.Services.AddSingleton(bc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BlobStorage.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);
    var credential = new DefaultAzureCredential();
    return new BlobServiceClient(new Uri(endpoint!), credential);
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
