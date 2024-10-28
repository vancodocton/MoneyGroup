namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Order not found.
/// </summary>
public class OrderNotFoundException
    : Exception
{
    public OrderNotFoundException() : base("Order not found")
    {
    }
}