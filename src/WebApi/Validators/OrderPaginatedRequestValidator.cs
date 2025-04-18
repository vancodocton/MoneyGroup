using FluentValidation;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.WebApi.Endpoints;

namespace MoneyGroup.WebApi.Validators;

public class OrderPaginatedRequestValidator : AbstractValidator<OrderPaginatedRequest>
{
    public OrderPaginatedRequestValidator(IValidator<IPaginatedOptions> paginatedOptionsValidator)
    {
        RuleFor(r => r.BuyerId)
            .GreaterThan(0);

        RuleFor(r => r.ParticipantId)
            .GreaterThan(0);

        RuleFor(r => r.TotalMax)
            .GreaterThan(0);

        RuleFor(r => r.TotalMin)
            .GreaterThan(0);

        When(r => r.TotalMin > 0 && r.TotalMax > 0, () =>
        {
            RuleFor(r => r.TotalMin)
                .LessThan(r => r.TotalMax);
        });

        Include(paginatedOptionsValidator);
    }
}
