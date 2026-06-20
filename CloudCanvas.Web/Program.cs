using Azure.Identity;
using Azure.Storage.Blobs;
using CloudCanvas.Shared;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<BlobStorageService>();
builder.Services.AddSingleton(bsc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BlobStorage.Self);
    Validate.StringValue(nameof(endpoint), endpoint);
    return new BlobServiceClient(new Uri(endpoint!), new DefaultAzureCredential());
});

builder.Services.AddTransient<CosmosClientWrapper>();
builder.Services.AddSingleton(cc =>
{
    var endpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);
    Validate.StringValue(nameof(endpoint), endpoint);
    return new CosmosClient(endpoint,new DefaultAzureCredential(), new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
