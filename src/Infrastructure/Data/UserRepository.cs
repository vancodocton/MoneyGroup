using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;

namespace MoneyGroup.Infrastructure.Data;

public sealed class UserRepository
    : EfRepository<User>
    , IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}
