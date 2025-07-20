using Azure.Storage.Blobs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<BlobStorageService>();
builder.Services.AddSingleton(bsc =>
{
    var blobStorageConnectionString = Environment.GetEnvironmentVariable(Secrets.MNSTRG);
    Validate.StringValue(nameof(blobStorageConnectionString), blobStorageConnectionString);
    return new BlobServiceClient(blobStorageConnectionString);
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
