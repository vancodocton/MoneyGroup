namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Order not found.
/// </summary>
public class OrderNotFoundException
    : BussinessValidationException
{
    public OrderNotFoundException() : base("Order not found")
    {
    }
}