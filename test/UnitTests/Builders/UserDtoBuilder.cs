using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.UnitTests.Builders;

/// <summary>
/// Builds <see cref="UserDto"/> instances for tests.
/// </summary>
public sealed class UserDtoBuilder
{
    private int _id = 1;
    private string _name = "User";
    private string? _email = "user@domain.com";

    public static UserDtoBuilder Valid() => new();

    public UserDtoBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UserDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserDtoBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public UserDto Build() => new()
    {
        Id = _id,
        Name = _name,
        Email = _email,
    };
}
