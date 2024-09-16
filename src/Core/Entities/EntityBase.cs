using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Entities;

public class EntityBase : IEntity<int>
{
    public int Id { get; set; }
}