using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Models.Paginations;

public class PaginatedOptions
   : IPaginatedOptions
{
    public PaginatedOptions(int page, int size)
    {
        Page = page;
        Size = size;
    }

    public virtual int Page { get; init; }

    public virtual int Size { get; init; }
}