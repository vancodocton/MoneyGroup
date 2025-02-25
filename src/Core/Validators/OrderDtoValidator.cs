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
            .NotEmpty();

        RuleForEach(o => o.Participants)
            .NotNull()
            .SetValidator(participantDtoValidator);
    }
}