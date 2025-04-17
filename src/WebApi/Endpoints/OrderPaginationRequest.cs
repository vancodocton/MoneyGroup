using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.WebApi.Endpoints;

public sealed class OrderPaginatedRequest
    : OrderPaginatedOptions
{
    public OrderPaginatedRequest(
        int? buyerId,
        int? participantId,
        decimal? totalMax,
        decimal? totalMin,
        int page = 1,
        int size = 10)
        : base(buyerId, participantId, totalMax, totalMin, page, size)
    {
    }

    [FromQuery(Name = "buyerId")]
    public override int? BuyerId { get; init; }

    [FromQuery(Name = "participantId")]
    public override int? ParticipantId { get; init; }

    [FromQuery(Name = "totalMax")]
    public override decimal? TotalMax { get; init; }

    [FromQuery(Name = "totalMin")]
    public override decimal? TotalMin { get; init; }

    [FromQuery(Name = "p")]
    [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageNumberNotPositiveErrorMessage)]
    public override int Page { get; init; } = 1;

    [FromQuery(Name = "s")]
    [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageSizeNotPositiveErrorMessage)]
    public override int Size { get; init; } = 10;
}
