using CloudCanvas.Shared.Constants;
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
    var CosmosConnString = Environment.GetEnvironmentVariable(Secrets.MTSTRG);
    Validate.StringValue(nameof(CosmosConnString), CosmosConnString);
    return new CosmosClient(CosmosConnString, new CosmosClientOptions
    {
        SerializerOptions  = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });
});
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
