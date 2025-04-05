namespace MoneyGroup.Core.Abstractions;

public interface IMapper
{
    TTarget Map<TTarget>(object source);

    IQueryable<TTarget> Project<TTarget>(IQueryable source);
}