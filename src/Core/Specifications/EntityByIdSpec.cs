using Ardalis.Specification;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Specifications;
public class EntityByIdSpec<TEntity>
    : Specification<TEntity>
    where TEntity : class, IEntity<int>
{
    public EntityByIdSpec(int id)
    {
        Query.Where(o => o.Id == id);
    }
}