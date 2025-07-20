using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);
builder.Services.AddTransient<BlobStorageService>(); // Inject custom Blob Storage Service
builder.Services.AddSingleton<CosmosClientWrapper>(); // Inject custom Cosmos wrapper
builder.Services.AddSingleton(bsc =>
{
    var blobStorageConnectionString = Environment.GetEnvironmentVariable(Secrets.MNSTRG);
    Validate.StringValue(nameof(blobStorageConnectionString), blobStorageConnectionString);
    return new BlobServiceClient(blobStorageConnectionString);
});

builder.Services.AddTransient<ServiceBusAdapter>(); // Inject my Service Bus Adapter
builder.Services.AddSingleton<IServiceBusClientFactory>(sp =>
{   // This factory should dynamically return the right ServiceBus client (listen vs read)
    var senderConnectionString = Environment.GetEnvironmentVariable(Secrets.FUMSGO);
    var listenerConnectionString = Environment.GetEnvironmentVariable(Secrets.FUMSGI);
    Validate.StringValue(nameof(senderConnectionString), senderConnectionString);     // Quick Validation even in startup, Fail-Fast principle
    Validate.StringValue(nameof(listenerConnectionString), listenerConnectionString);
    var sender = new ServiceBusClient(senderConnectionString);
    var listener = new ServiceBusClient(listenerConnectionString);
    return new SBClientFactory(sender, listener);   // If it throws at this point, I leave it up to AppInsights and Telemetry
});


builder.Services.Configure<JsonSerializerOptions>(options =>
{   // Industry standard for json messaging is camelCase, System.Text.Json uses PascalCase by default.
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddLogging();
builder.Build().Run();
