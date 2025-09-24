using System.Diagnostics;

using Aspire.Hosting;
using Aspire.Hosting.Testing;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MoneyGroup.Infrastructure.Data;

using Xunit.Sdk;
using Xunit.v3;

namespace MoneyGroup.IntegrationTests.Fixtures;

public sealed class ApplicationDbContextFactory
    : DbContextFactory<ApplicationDbContext>
    , IAsyncLifetime
{
    private readonly IMessageSink _diagnosticMessageSink;

    public ApplicationDbContextFactory(IMessageSink diagnosticMessageSink)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
    }

    private DistributedApplication? _app;
    private string? _connectionString;

    public async ValueTask InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MoneyGroup_AppHost>();

        _app = await appHost.BuildAsync().ConfigureAwait(false);

        _connectionString = await _app.GetConnectionStringAsync("SqlServerConnection").ConfigureAwait(false);
    }

    private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = _connectionString
            ?? throw new InvalidOperationException();
        optionsBuilder.UseSqlServer(connectionString);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(message =>
        {
            _diagnosticMessageSink.OnMessage(new DiagnosticMessage(message));
            Debug.WriteLine(message);
        }, LogLevel.Information);

        return optionsBuilder.Options;
    }

    protected override ApplicationDbContext CreateDbContextCore()
    {
        return new ApplicationDbContext(GetDbContextOptions());
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync().ConfigureAwait(false);
        }

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
