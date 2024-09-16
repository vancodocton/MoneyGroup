using Microsoft.Extensions.DependencyInjection;

namespace MoneyGroup.Infrastucture.AutoMapper.Profiles;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperExtensions).Assembly);
        return services;
    }
}