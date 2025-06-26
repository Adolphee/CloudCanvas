using CloudCanvas.Shared.Constants;
using CloudCanvas.Functions.Services;
using CloudCanvas.Services;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.Configure<BlobStorageService>(builder.Configuration.GetSection(BlobStorage.Self)); // For blob storage connection secrets
builder.Services.AddTransient<ServiceBusAdapter>(); // Inject my Service Bus Adapter
builder.Services.AddSingleton<ServiceBusClientFactory>(); // To dynamically return the right ServiceBus client (listen vs read)
builder.Services.AddTransient<BlobMetaConverter>(); // Conoverts blob metadata from a valid json message to {ExtractMetadataMessageDTO}
builder.Services.Configure<JsonSerializerOptions>(options =>
{   // Industry standard for json messaging is camelCase, System.Text.Json uses PascalCase by default.
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; 
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddTransient<BlobStorageService>(); // Inject custom Blob Storage Service
builder.Services.AddLogging();
builder.Build().Run();
