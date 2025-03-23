using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MoneyGroup.Infrastructure.Data;

namespace MoneyGroup.Infrastructure.SqlServer;
public static class DependenciesExtensions
{
    private static string? MigrationsAssembly => typeof(DependenciesExtensions).Assembly.GetName().Name;

    public static IServiceCollection AddApplicationDbContextSqlServer(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, config =>
            {
                config.MigrationsAssembly(MigrationsAssembly);
            }));

        return services;
    }
}