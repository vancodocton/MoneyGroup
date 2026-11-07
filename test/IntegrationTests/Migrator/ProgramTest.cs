using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MoneyGroup.Infrastructure.Data;
using MoneyGroup.Infrastructure.SqlServer;
using MoneyGroup.IntegrationTests.Fixtures;

namespace MoneyGroup.IntegrationTests.Migrator;

public class ProgramTest
    : IClassFixture<SqlServerContainerFixture>
{
    private readonly string _connectionString;

    public ProgramTest(SqlServerContainerFixture fixture)
    {
        _connectionString = fixture.CreateDatabaseConnectionString();
    }

    [Fact]
    public async Task Main_NewDatabase_ShouldApplyAllMigrations()
    {
        // Act
        await RunMigratorAsync();

        // Assert
        await AssertAllMigrationsAppliedAsync();
    }

    [Fact]
    public async Task Main_MigratedDatabase_ShouldKeepAllMigrationsApplied()
    {
        // Arrange
        await RunMigratorAsync();

        // Act
        await RunMigratorAsync();

        // Assert
        await AssertAllMigrationsAppliedAsync();
    }

    private async Task RunMigratorAsync()
    {
        var entryPoint = Assembly.Load("MoneyGroup.Migrator").EntryPoint
            ?? throw new InvalidOperationException();
        string[] args = [$"--ConnectionStrings:SqlServerConnection={_connectionString}"];

        // The entry point blocks on async work, so keep it off xUnit's synchronization context.
        await Task.Run(() => entryPoint.Invoke(null, [args]), TestContext.Current.CancellationToken);
    }

    private async Task AssertAllMigrationsAppliedAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var services = new ServiceCollection()
            .AddApplicationDbContextSqlServer(_connectionString)
            .BuildServiceProvider();
        await using var scope = services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database;

        var expected = database.GetMigrations();
        Assert.NotEmpty(expected);
        Assert.Equal(expected, await database.GetAppliedMigrationsAsync(cancellationToken));
    }
}
