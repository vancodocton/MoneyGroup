using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Specifications;

public class EntityByIdsSpec<TEntity>
    : Specification<TEntity>
    where TEntity : class, IEntity<int>
{
    public EntityByIdsSpec(IEnumerable<int> ids)
    {
        var idsList = ids.ToList();
        Query.Where(e => idsList.Contains(e.Id));
    }
}
