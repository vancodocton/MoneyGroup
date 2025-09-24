using Microsoft.EntityFrameworkCore;

namespace MoneyGroup.IntegrationTests.Fixtures;

public sealed class SQLiteDbContextFactory
    : DbContextFactory<SQLiteDbContext>
{
    protected override SQLiteDbContext CreateDbContextCore()
    {
        var options = new DbContextOptionsBuilder<SQLiteDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var dbContext = new SQLiteDbContext(options);
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}

public class SQLiteDbContext : DbContext
{
    public SQLiteDbContext(DbContextOptions<SQLiteDbContext> options)
        : base(options)
    {
    }

    public DbSet<SimpleEntity> SimpleEntities { get; set; } = null!;
}
