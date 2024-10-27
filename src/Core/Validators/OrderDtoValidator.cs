using FluentValidation;

using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.Core.Validators;

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator(IValidator<ConsumerDto> consumerDtoValidator)
    {
        RuleFor(o => o.Title)
            .NotEmpty();

        RuleFor(o => o.IssuerId)
            .GreaterThan(0);

        RuleFor(o => o.Total)
            .GreaterThanOrEqualTo(0);

        RuleFor(o => o.Consumers)
            .NotNull();

        RuleForEach(o => o.Consumers)
            .NotNull()
            .SetValidator(consumerDtoValidator);
    }
}