namespace MoneyGroup.Core.Exceptions;

/// <inheritdoc />
public abstract class BussinessValidationException
    : Exception
{
    /// <inheritdoc />
    protected BussinessValidationException(string? message) : base(message)
    {
    }
}