using Azure.Identity;
using Azure.Storage.Blobs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Cosmos;
using System.Collections;
using CloudCanvas.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CCDBContext") ?? throw new InvalidOperationException("Connection string 'CCDBContext' not found.");
builder.Services.AddRazorPages();

builder.Services.AddDbContext<CCDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<CCDBContext>();

// Add services to the container.
builder.Services.AddTransient<BlobStorageService>();
builder.Services.AddSingleton(bsc =>
{
    var endpoint = Environment.GetEnvironmentVariable(BlobStorage.Uri);
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
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
