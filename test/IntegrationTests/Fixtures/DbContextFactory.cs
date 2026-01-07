using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Infrastructure.Mapperly;

namespace MoneyGroup.IntegrationTests.Fixtures;

public abstract class NonGenericDbContextFactory
{
    protected static readonly IMapper StaticMapper = new Mapper();

    protected static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddUserSecrets<NonGenericDbContextFactory>()
        .Build();
}

public abstract class DbContextFactory<TDbContext>
    : NonGenericDbContextFactory, IAsyncDisposable
    where TDbContext : DbContext
{

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public IMapper Mapper { get; private set; } = StaticMapper;

    private TDbContext? _dbContext;

    protected abstract TDbContext CreateDbContextCore();

    public TDbContext CreateDbContext(bool isCachedInstance = false)
    {
        if (isCachedInstance)
        {
            _semaphoreSlim.Wait();
            _dbContext ??= CreateDbContextCore();
            _semaphoreSlim.Release();
            return _dbContext;
        }

        return CreateDbContextCore();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync().ConfigureAwait(false);
        }
        _semaphoreSlim.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
