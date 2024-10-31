namespace MoneyGroup.WebApi.Endpoints;

public class OrderPaginationRequest
    : BasePaginationRequest
{
    public OrderPaginationRequest(int page = 1, int size = 10)
        : base(page, size)
    {
    }
}