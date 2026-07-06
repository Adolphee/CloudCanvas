
using Azure.Identity;
using CloudCanvas.Application.Abstractions.Cosmos;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure;
using CloudCanvas.Infrastructure.Cosmos;
using CloudCanvas.Infrastructure.Persistence;
using CloudCanvas.Infrastructure.Persistence.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
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
builder.Services.AddTransient(typeof(IPostsRepositoryCosmos<>), typeof(CosmosClientWrapper<>));

builder.Services.AddTransient<CosmosClientWrapper<Post>>();
builder.Services.AddTransient<IPhotoRepositoryEF, PhotoRepositoryEF>();
builder.Services.AddDbContext<CCDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(SQLServer.ConnectionString));
});
var config = TypeAdapterConfig.GlobalSettings;
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, Mapper>();

var options = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

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
