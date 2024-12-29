namespace MoneyGroup.Core.Abstractions;

public interface IPaginationOptions
{
    public int Page { get; }

    public int Size { get; }
}