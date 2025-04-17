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
                    var idsHashSet = new HashSet<int>(l.Count());
                    foreach (var participantId in l.Select(c => c.ParticipantId))
                    {
                        if (!idsHashSet.Add(participantId))
                        {
                            return false;
                        }
                    }
                    return true;
                })
                .WithMessage("Duplicated participant");
            });
    }
}
