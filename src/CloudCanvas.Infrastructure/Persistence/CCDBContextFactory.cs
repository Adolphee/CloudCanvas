using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CloudCanvas.Infrastructure.Persistence;

public sealed class CCDBContextFactory : IDesignTimeDbContextFactory<CCDBContext>
{
    public CCDBContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../CloudCanvas.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("localdb");

        var optionsBuilder = new DbContextOptionsBuilder<CCDBContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new CCDBContext(optionsBuilder.Options);
    }
}
