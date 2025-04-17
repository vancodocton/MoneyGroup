namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Participant not found.
/// </summary>
public class ParticipantNotFoundException
    : BusinessValidationException
{
    public ParticipantNotFoundException() : base("Participant not found")
    {
    }
}
