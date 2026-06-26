using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using CloudCanvas.Application.Common;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure.BlobStorage;
using CloudCanvas.Infrastructure.Cosmos;
using CloudCanvas.Infrastructure.Messaging;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddTransient<CosmosClientWrapper<IPost>>();
builder.Services.AddTransient<BlobStorageService>();
builder.Services.AddSingleton(bsc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BStorage.Uri);
    return new BlobServiceClient(new Uri(endpoint!), new DefaultAzureCredential());
});

builder.Services.AddTransient<ServiceBusAdapter>(); // Inject my Service Bus Adapter
builder.Services.AddSingleton<IServiceBusClientFactory>(sp =>
{  
    var endpoint = Environment.GetEnvironmentVariable(ServiceBus.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);     // Quick Validation even in startup, Fail-Fast principle
    var client = new ServiceBusClient(endpoint, new DefaultAzureCredential());
    return new SBClientFactory(client);
});

builder.Services.AddSingleton(sp =>
{
    var endpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);
    return new CosmosClient(endpoint, new DefaultAzureCredential(),
        new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
});

builder.Services.Configure<JsonSerializerOptions>(options =>
{   // Industry standard for json messaging is camelCase, System.Text.Json uses PascalCase by default.
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddDurableTaskClient();

builder.Build().Run();
