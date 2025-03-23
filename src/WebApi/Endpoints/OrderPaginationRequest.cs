using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.WebApi.Endpoints;

public sealed class OrderPaginatedRequest
    : PaginatedOptions
{
    public OrderPaginatedRequest(
        [FromQuery(Name = "p")]
        [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageNumberNotPositiveErrorMessage)]
        int page = 1,
        [FromQuery(Name = "s")]
        [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageSizeNotPositiveErrorMessage)]
        int size = 10)
        : base(page, size)
    {
    }
}