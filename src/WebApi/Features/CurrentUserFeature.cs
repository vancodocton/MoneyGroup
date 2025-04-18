using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.WebApi.Features;

public class CurrentUserFeature : ICurrentUserFeature
{
    public UserDto? User { get; set; }
}
