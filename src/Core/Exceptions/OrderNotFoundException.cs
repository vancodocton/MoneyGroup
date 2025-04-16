namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Order not found.
/// </summary>
public class OrderNotFoundException
    : BusinessValidationException
{
    public OrderNotFoundException() : base("Order not found")
    {
    }
}