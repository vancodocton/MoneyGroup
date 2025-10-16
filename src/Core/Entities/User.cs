namespace MoneyGroup.Core.Entities;

public class User
    : EntityBase
{
    public string Name { get; set; } = null!;

    public string? Email { get; set; }
}
