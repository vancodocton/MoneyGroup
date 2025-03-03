namespace MoneyGroup.Core.Exceptions;

/// <summary>
/// The exception that is thrown when Duplicated participant.
/// </summary>
public class ParticipantDuplicatedException
    : BussinessValidationException
{
    public ParticipantDuplicatedException() : base("Duplicated participant")
    {
    }
}