using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

// BLOB STORAGE
builder.Services.AddTransient<BlobStorageService>(); // Inject custom Blob Storage Service
builder.Services.AddSingleton(cc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BlobStorage.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);
    return new BlobServiceClient(new Uri(endpoint!), new DefaultAzureCredential());
});

// COSMOS DB
builder.Services.AddTransient<CosmosClientWrapper>();
builder.Services.AddSingleton(bsc =>
{
    var endpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);
    return new CosmosClient(endpoint, new DefaultAzureCredential(), new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });
});

// SERVICE BUS
builder.Services.AddTransient<ServiceBusAdapter>(); // Inject my Service Bus Adapter
builder.Services.AddSingleton<IServiceBusClientFactory>(sp =>
{ 
    var endpoint = Environment.GetEnvironmentVariable(ServiceBus.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);     // Quick Validation even in startup, Fail-Fast principle
    var client = new ServiceBusClient(endpoint, new DefaultAzureCredential()); // Entra ID authentication
    return new SBClientFactory(client);
});

// JSON Serializer Options
builder.Services.Configure<JsonSerializerOptions>(options =>
{   // Industry standard for json messaging is camelCase, System.Text.Json uses PascalCase by default.
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// APPLICATION INSIGHTS
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddLogging();
builder.Build().Run();
