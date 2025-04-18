using MoneyGroup.Core.Models.Paginations;

namespace MoneyGroup.Core.Models.Users;

public class UserPaginatedOptions
    : PaginatedOptions
{
    public UserPaginatedOptions(int page, int size) : base(page, size)
    {
    }

    public virtual string? Keyword { get; init; }
}
