using Microsoft.Extensions.DependencyInjection;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Infrastructure;

public static class MapperExtensions
{
    public static IQueryable<TTarget> ProjectTo<TTarget>(this IQueryable source, IMapper mapper)
    {
        return mapper.Project<TTarget>(source);
    }

    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, Mapper>();

        return services;
    }
}