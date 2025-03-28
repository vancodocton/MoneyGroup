using Ardalis.Specification;

using MoneyGroup.Core.Entities;

namespace MoneyGroup.Core.Specifications;

public class UserByEmailSpec : Specification<User>
{
    public UserByEmailSpec(string email)
    {
        Query.Where(user => user.Email == email);
    }
}