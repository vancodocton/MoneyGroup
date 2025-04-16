namespace MoneyGroup.Core.Exceptions;

/// <inheritdoc />
public abstract class BusinessValidationException
    : Exception
{
    /// <inheritdoc />
    protected BusinessValidationException(string? message) : base(message)
    {
    }
}