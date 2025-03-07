namespace MoneyGroup.Core.Abstractions;

public interface IPaginatedOptions
{
    public int Page { get; }

    public int Size { get; }
}