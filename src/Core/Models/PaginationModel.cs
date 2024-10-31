namespace MoneyGroup.Core.Models;

public class PaginationModel<T>
{
    public required int Page { get; init; }

    public required int Count { get; init; }

    public required int Total { get; init; }

    public required IEnumerable<T> Items { get; init; }
}