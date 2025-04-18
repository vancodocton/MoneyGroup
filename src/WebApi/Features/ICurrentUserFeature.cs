using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.WebApi.Features;

public interface ICurrentUserFeature
{
    public UserDto? User { get; }
}
