namespace MoneyGroup.Core.Abstractions;

public interface IEntity<TKey>
{
    public TKey Id { get; set; }
}
// CI test for AC 2: C# change. Do not merge.
