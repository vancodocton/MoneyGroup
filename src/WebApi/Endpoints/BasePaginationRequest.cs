using Microsoft.AspNetCore.Mvc;

namespace MoneyGroup.WebApi.Endpoints;

public abstract class BasePaginationRequest
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