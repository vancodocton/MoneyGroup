using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Infrastructure;

public partial class Mapper
    : IMapper
{
    public partial TTarget Map<TTarget>(object source);

    public partial IQueryable<TTarget> Project<TTarget>(IQueryable source);
}