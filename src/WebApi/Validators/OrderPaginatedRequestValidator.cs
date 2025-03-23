using FluentValidation;

using MoneyGroup.Core.Validators;
using MoneyGroup.WebApi.Endpoints;

namespace MoneyGroup.WebApi.Validators;

public class OrderPaginatedRequestValidator : AbstractValidator<OrderPaginatedRequest>
{
    public OrderPaginatedRequestValidator()
    {
        Include(new PaginatedOptionsValidator());
    }
}