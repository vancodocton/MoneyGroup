using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MoneyGroup.Infrastructure.Data;

namespace MoneyGroup.IntegrationTests.Fixtures;

public sealed class ApplicationDbContextFactory
    : DbContextFactory<ApplicationDbContext>
{
    private static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = Configuration.GetConnectionString("SqlServerConnection")
            ?? throw new InvalidOperationException();
        optionsBuilder.UseSqlServer(connectionString);
        return optionsBuilder.Options;
    }

    protected override ApplicationDbContext CreateDbContextCore()
    {
        return new ApplicationDbContext(GetDbContextOptions());
    }
}