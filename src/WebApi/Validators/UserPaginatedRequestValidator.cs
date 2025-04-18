using FluentValidation;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.WebApi.Endpoints;

namespace MoneyGroup.WebApi.Validators;

public class UserPaginatedRequestValidator
    : AbstractValidator<UserPaginatedRequest>
{
    public UserPaginatedRequestValidator(IValidator<IPaginatedOptions> paginatedOptionsValidator)
    {
        Include(paginatedOptionsValidator);
    }
}
