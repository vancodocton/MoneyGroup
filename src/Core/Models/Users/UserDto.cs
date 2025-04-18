namespace MoneyGroup.Core.Models.Users;
public class UserDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Email { get; set; }
}
