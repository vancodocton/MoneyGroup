namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Issuer not found.
/// </summary>
public class IssuerNotFoundException
    : BussinessValidationException
{
    public IssuerNotFoundException() : base("Issuer not found")
    {
    }
}