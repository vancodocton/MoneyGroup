using AutoMapper;

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

    public Task<bool> AnyAsync(int id, CancellationToken cancellationToken = default)
    {
        return AnyAsync(x => x.Id == id, cancellationToken);
    }
}