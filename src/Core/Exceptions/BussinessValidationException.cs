namespace MoneyGroup.Core.Exceptions;

/// <inheritdoc />
public abstract class BussinessValidationException
    : Exception
{
    /// <inheritdoc />
    public BussinessValidationException(string? message) : base(message)
    {
    }
}
