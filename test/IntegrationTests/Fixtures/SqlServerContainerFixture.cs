using Microsoft.Data.SqlClient;

using Testcontainers.MsSql;

namespace MoneyGroup.IntegrationTests.Fixtures;

/// <summary>
/// Starts a throwaway SQL Server container that holds no application data.
/// </summary>
public sealed class SqlServerContainerFixture
    : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    public string CreateDatabaseConnectionString()
    {
        return new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = $"MigratorTest_{Guid.NewGuid():N}",
        }.ConnectionString;
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
