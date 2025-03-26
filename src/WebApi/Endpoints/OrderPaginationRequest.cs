using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.WebApi.Endpoints;

public sealed class OrderPaginatedRequest
    : OrderPaginatedOptions
{
    public OrderPaginatedRequest(
        [FromQuery(Name = "buyerId")]
        int? buyerId,
        [FromQuery(Name = "participantId")]
        int? participantId,
        [FromQuery(Name = "totalMax")]
        decimal? totalMax,
        [FromQuery(Name = "totalMin")]
        decimal? totalMin,
        [FromQuery(Name = "p")]
        [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageNumberNotPositiveErrorMessage)]
        int page = 1,
        [FromQuery(Name = "s")]
        [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageSizeNotPositiveErrorMessage)]
        int size = 10)
        : base(buyerId, participantId, totalMax, totalMin, page, size)
    {
    }
}