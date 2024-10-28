namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Issuer not found.
/// </summary>
public class IssuerNotFoundException
    : Exception
{
    public IssuerNotFoundException() : base("Issuer not found")
    {
    }
}