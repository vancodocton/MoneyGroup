using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Models.Users;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.WebApi.Endpoints;

public class UserPaginatedRequest
    : UserPaginatedOptions
{
    public UserPaginatedRequest([System.Diagnostics.CodeAnalysis.AllowNull] string keyword, int page = 1, int size = 10) : base(page, size)
    {
        Keyword = keyword;
    }

    [FromQuery(Name = "q")]
    public override string? Keyword { get; init; }

    [FromQuery(Name = "p")]
    public override int Page { get; init; }

    [FromQuery(Name = "s")]
    [Range(1, int.MaxValue, ErrorMessage = PaginatedOptionsValidator.PageSizeNotPositiveErrorMessage)]
    public override int Size { get; init; }
}
