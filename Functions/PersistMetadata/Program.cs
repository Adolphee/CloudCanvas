
using Azure.Messaging.ServiceBus;
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
builder.Services.AddSingleton<CosmosClientWrapper>(); // Inject custom Cosmos wrapper

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

builder.Services.AddSingleton(sp =>
{
    var cosmosConnectionString = Environment.GetEnvironmentVariable(Secrets.MTSTRG);
    Validate.StringValue(nameof(cosmosConnectionString), cosmosConnectionString);
    return new CosmosClient(cosmosConnectionString, 
        new CosmosClientOptions {
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
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddLogging();
builder.Build().Run();
