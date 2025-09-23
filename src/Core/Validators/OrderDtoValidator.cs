using FluentValidation;

using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Validators;

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator(IValidator<ParticipantDto> participantDtoValidator)
    {
        RuleFor(o => o.Title)
            .NotEmpty();

        RuleFor(o => o.BuyerId)
            .GreaterThan(0);

        RuleFor(o => o.Total)
            .GreaterThanOrEqualTo(0);

        RuleFor(o => o.Participants)
            .NotEmpty()
            .ForEach(o => o
                .NotNull()
                .SetValidator(participantDtoValidator)
            ).DependentRules(() =>
            {
                RuleFor(o => o.Participants)
                .Must(l =>
                {
                    // Use LINQ Where to find duplicates
                    return l.Select(c => c.ParticipantId)
                            .Distinct()
                            .Count() == l.Count();
                })
                .WithMessage("Duplicated participant");
            });
    }
}
