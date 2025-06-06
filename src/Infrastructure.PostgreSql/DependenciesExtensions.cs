﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MoneyGroup.Infrastructure.Data;

namespace MoneyGroup.Infrastructure.PostgreSql;
public static class DependenciesExtensions
{
    private static string? MigrationsAssembly => typeof(DependenciesExtensions).Assembly.GetName().Name;

    public static IServiceCollection AddApplicationDbContextNpgsql(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, config =>
            {
                config.MigrationsAssembly(MigrationsAssembly);
            }));

        return services;
    }
}
