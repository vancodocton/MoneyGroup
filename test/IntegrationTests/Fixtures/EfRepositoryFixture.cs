using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MoneyGroup.Infrastructure.AutoMapper.Profiles;
using MoneyGroup.Infrastructure.Data;

namespace MoneyGroup.IntegrationTests.Fixtures;

public sealed class EfRepositoryFixture
    : IDisposable
{
    private static readonly IMapper MapperConfiguration = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(AutoMapperExtensions).Assembly);
    }).CreateMapper();

    public IMapper Mapper { get; private set; } = MapperConfiguration;

    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .AddUserSecrets<EfRepositoryFixture>()
        .Build();

    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    private ApplicationDbContext? _dbContext;

    private static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = Configuration.GetConnectionString("SqlServerConnection")
            ?? throw new InvalidOperationException();
        optionsBuilder.UseSqlServer(connectionString);
        return optionsBuilder.Options;
    }

    public ApplicationDbContext CreateDbContext(bool isCachedInstance = false)
    {
        if (isCachedInstance)
        {
            SemaphoreSlim.Wait();
            _dbContext ??= new ApplicationDbContext(GetDbContextOptions());
            SemaphoreSlim.Release();
            return _dbContext;
        }

        return new ApplicationDbContext(GetDbContextOptions());
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
        _dbContext = null;
    }
}