﻿using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MoneyGroup.Infrastucture.AutoMapper.Profiles;
using MoneyGroup.Infrastucture.Data;

namespace MoneyGroup.IntegrationTests.Fixtures;

public sealed class EfRepositoryFixture
    : IDisposable
{
    private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(typeof(AutoMapperExtensions).Assembly);
    }).CreateMapper();

    public IMapper Mapper { get; private set; } = _mapper;

    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddUserSecrets<EfRepositoryFixture>()
        .Build();

    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    private ApplicationDbContext? _dbContext;

    private static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = Configuration.GetConnectionString("PostgreSqlConnection")
            ?? throw new InvalidOperationException();
        optionsBuilder.UseNpgsql(connectionString);
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