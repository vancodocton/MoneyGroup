using AutoMapper.EquivalencyExpression;

using Microsoft.Extensions.DependencyInjection;

namespace MoneyGroup.Infrastructure.AutoMapper.Profiles;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddCollectionMappers();
        }, typeof(AutoMapperExtensions).Assembly);
        return services;
    }
}