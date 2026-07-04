using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CloudCanvas.Infrastructure.Persistence;

/// <summary>
/// Creates CCDBContext for EF Core design-time operations such as migrations.
/// Because this project is a class library, EF tools can't instantiate it directly at runtime,
/// so this factory loads config from the API project and constructs the context manually.
/// </summary>
public sealed class CCDBContextFactory : IDesignTimeDbContextFactory<CCDBContext>
{
    public CCDBContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../CloudCanvas.Api");

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var conn_str = config.GetConnectionString("localdb");

        var optionsBuilder = new DbContextOptionsBuilder<CCDBContext>();
        optionsBuilder.UseSqlServer(conn_str);

        return new CCDBContext(optionsBuilder.Options);
    }
}
