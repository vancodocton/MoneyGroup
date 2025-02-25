namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Buyer not found.
/// </summary>
public class BuyerNotFoundException
    : BussinessValidationException
{
    public BuyerNotFoundException() : base("Buyer not found")
    {
    }
}