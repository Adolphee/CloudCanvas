using CloudCanvas.Interfaces;
using CloudCanvas.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.Configure<BlobStorageService>(builder.Configuration.GetSection("AzureBlobStorage"));
builder.Services.AddTransient<IBlobStorageService, BlobStorageService>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.AddTransient<BlobStorageService>();

builder.Services.AddLogging();
builder.Build().Run();
