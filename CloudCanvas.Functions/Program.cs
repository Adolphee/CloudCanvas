using Azure.Messaging.ServiceBus;
using CloudCanvas.Constants;
using CloudCanvas.Interfaces;
using CloudCanvas.Services;
using CloudCanvas.Shared;
using CloudCanvas.Shared.Config;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.Configure<BlobStorageService>(builder.Configuration.GetSection(BlobStorage.Self));
builder.Services.Configure<Dictionary<string, Topic>>(builder.Configuration.GetSection(ServiceBus.Self).GetSection("Topics"));
builder.Services.Configure<ServiceBusOptions>(builder.Configuration.GetSection(ServiceBus.Self)); // Inject my ServiceBus config class
builder.Services.AddTransient<ServiceBusAdapter>(); // Inject my Service Bus Adapter
builder.Services.AddSingleton<ServiceBusClientFactory>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddTransient<BlobStorageService>(); // Inject custom Blob Storage Service
builder.Services.AddLogging();
builder.Build().Run();
