namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Consumer not found.
/// </summary>
public class ConsumerNotFoundException
    : Exception
{
    public ConsumerNotFoundException() : base("Consumer not found")
    {
    }
}