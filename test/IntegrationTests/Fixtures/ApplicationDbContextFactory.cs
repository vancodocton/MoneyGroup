using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using MoneyGroup.Infrastructure.Data;

using Xunit.Sdk;
using Xunit.v3;

namespace MoneyGroup.IntegrationTests.Fixtures;

public sealed class ApplicationDbContextFactory
    : DbContextFactory<ApplicationDbContext>
{
    private readonly IMessageSink _diagnosticMessageSink;

    public ApplicationDbContextFactory(IMessageSink diagnosticMessageSink)
    {
        _diagnosticMessageSink = diagnosticMessageSink;
    }

    private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = Configuration.GetConnectionString("SqlServerConnection")
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
}
