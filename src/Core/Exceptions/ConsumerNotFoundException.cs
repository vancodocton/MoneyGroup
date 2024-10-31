namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Consumer not found.
/// </summary>
public class ConsumerNotFoundException
    : BussinessValidationException
{
    public ConsumerNotFoundException() : base("Consumer not found")
    {
    }
}