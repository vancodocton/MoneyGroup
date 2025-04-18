using Ardalis.Specification;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.Core.Specifications;

public class UserPaginatedSpec : BasePaginatedSpecification<User>
{
    public UserPaginatedSpec(UserPaginatedOptions options) : base(options)
    {
        if (!string.IsNullOrWhiteSpace(options.Keyword))
        {
#pragma warning disable S4058 // Overloads with a "StringComparison" parameter should be used
            Query.Where(x => x.Name.Contains(options.Keyword));
#pragma warning restore S4058 // Overloads with a "StringComparison" parameter should be used
        }
    }
}
