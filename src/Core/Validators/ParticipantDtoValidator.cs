using FluentValidation;
using FluentValidation.Results;

using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Validators;

public class ParticipantDtoValidator : AbstractValidator<ParticipantDto>
{
    public ParticipantDtoValidator()
    {
        RuleFor(c => c.ParticipantId)
            .GreaterThan(0);
    }

    public override Task<ValidationResult> ValidateAsync(ValidationContext<ParticipantDto> context, CancellationToken cancellation = default)
    {
        return base.ValidateAsync(context, cancellation);
    }
}