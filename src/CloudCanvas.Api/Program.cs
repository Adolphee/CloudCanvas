
using Azure.Identity;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Infrastructure;
using CloudCanvas.Infrastructure.Cosmos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            //.AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();

builder.Services.AddControllers();
builder.Services.AddTransient<IPost, Post>();
builder.Services.AddTransient(typeof(IPostsRepository<>), typeof(CosmosClientWrapper<>));

builder.Services.AddTransient<CosmosClientWrapper<Post>>();

builder.Services.AddSingleton( cc =>
{
    var endpoint = Environment.GetEnvironmentVariable(CloudCosmos.Uri);

    var settings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    //settings.Converters.Add(new PostJsonConverter());
    return new CosmosClient(endpoint, new DefaultAzureCredential(), new CosmosClientOptions
    {
        ConnectionMode = ConnectionMode.Gateway,
        Serializer = new CustomCosmosSerializer(settings, new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
        })
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
