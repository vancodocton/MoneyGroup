using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MoneyGroup.Infrastructure.AutoMapper.Profiles;

namespace MoneyGroup.IntegrationTests.Fixtures;

public abstract class NonGenericDbContextFactory
{
    protected static readonly IMapper StaticMapper = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(AutoMapperExtensions).Assembly);
    }).CreateMapper();

    protected static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddUserSecrets<NonGenericDbContextFactory>()
        .Build();
}

public abstract class DbContextFactory<TDbContext>
    : NonGenericDbContextFactory, IDisposable
    where TDbContext : DbContext
{

#pragma warning disable S2743 // Static fields should not be used in generic types
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
#pragma warning restore S2743 // Static fields should not be used in generic types

    public IMapper Mapper { get; private set; } = StaticMapper;

    private TDbContext? _dbContext;
    private bool _disposedValue;

    protected abstract TDbContext CreateDbContextCore();

    public TDbContext CreateDbContext(bool isCachedInstance = false)
    {
        if (isCachedInstance)
        {
            SemaphoreSlim.Wait();
            _dbContext ??= CreateDbContextCore();
            SemaphoreSlim.Release();
            return _dbContext;
        }

        return CreateDbContextCore();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}