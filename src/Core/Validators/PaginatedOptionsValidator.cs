using FluentValidation;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.Core.Validators;

public class PaginatedOptionsValidator : AbstractValidator<IPaginatedOptions>
{
    public const string PageNumberNotPositiveErrorMessage = "Page number must be a non-negative integer.";
    public const string PageSizeNotPositiveErrorMessage = "Page size must be a non-negative integer.";

    public PaginatedOptionsValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage(PageNumberNotPositiveErrorMessage);
        RuleFor(x => x.Size).GreaterThan(0).WithMessage(PageSizeNotPositiveErrorMessage);
    }
}