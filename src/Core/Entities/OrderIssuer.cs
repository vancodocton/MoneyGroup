namespace MoneyGroup.Core.Entities;
public class OrderIssuer
{
    public int IssuerId { get; set; }

    public int OrderId { get; set; }

    public User Issuer { get; set; } = null!;

    public Order Order { get; set; } = null!;
}