namespace MoneyGroup.Core.Abstractions;

public interface IEntity<TKey>
{
    public TKey Id { get; set; }
}