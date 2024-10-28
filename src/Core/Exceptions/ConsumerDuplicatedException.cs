namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Duplicated consumer.
/// </summary>
public class ConsumerDuplicatedException
    : Exception
{
    public ConsumerDuplicatedException() : base("Duplicated consumer")
    {
    }
}