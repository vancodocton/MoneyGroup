using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.WebApi.Endpoints;

public abstract class BasePaginationRequest
    : IPaginatedOptions
{
    protected BasePaginationRequest(int page, int size)
    {
        Page = page;
        Size = size;
    }

    [FromQuery(Name = "p")]
    [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageNumberNotPositiveErrorMessage)]
    public int Page { get; }

    [FromQuery(Name = "s")]
    [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageSizeNotPositiveErrorMessage)]
    public int Size { get; }
}