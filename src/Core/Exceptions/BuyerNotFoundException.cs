namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Buyer not found.
/// </summary>
public class BuyerNotFoundException
    : BusinessValidationException
{
    public BuyerNotFoundException() : base("Buyer not found")
    {
    }
}