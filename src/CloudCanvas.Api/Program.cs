
using Azure.Identity;
using CloudCanvas.Application;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure;
using CloudCanvas.Infrastructure.Cosmos;
using CloudCanvas.Infrastructure.Persistence;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using SQLServer = CloudCanvas.Application.Common.Constants.SQLServer;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            //.AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IPost, Post>();
builder.Services.AddTransient<IPhotoProjectionStore, PhotoProjectionStore>();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddDbContext<CCDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(SQLServer.ConnectionString));
});
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);

builder.Services.AddSingleton( cc =>
{
    var endpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);

    return new CosmosClient(endpoint, new DefaultAzureCredential(), new CosmosClientOptions
    {
        ConnectionMode = ConnectionMode.Gateway,
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
            IgnoreNullValues = true
        }
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
