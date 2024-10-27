using FluentValidation;
using FluentValidation.Results;

using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Validators;

public class ConsumerDtoValidator : AbstractValidator<ConsumerDto>
{
    public ConsumerDtoValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0);
    }

    public override Task<ValidationResult> ValidateAsync(ValidationContext<ConsumerDto> context, CancellationToken cancellation = default)
    {
        return base.ValidateAsync(context, cancellation);
    }
}