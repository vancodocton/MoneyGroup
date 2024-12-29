using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Abstractions;

namespace MoneyGroup.WebApi.Endpoints;

public abstract class BasePaginationRequest
    : IPaginationOptions
{
    protected BasePaginationRequest(int page, int size)
    {
        Page = page;
        Size = size;
    }

    [FromQuery(Name = "p")]
    public int Page { get; }

    [FromQuery(Name = "s")]
    public int Size { get; }
}