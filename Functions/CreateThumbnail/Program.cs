using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using CloudCanvas.Shared;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);
builder.Services.AddTransient<BlobStorageService>();
builder.Services.AddSingleton(bsc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BlobStorage.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);
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


builder.Services.Configure<JsonSerializerOptions>(options =>
{   // Industry standard for json messaging is camelCase, System.Text.Json uses PascalCase by default.
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddLogging();
builder.Build().Run();
