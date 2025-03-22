namespace MoneyGroup.WebApi.Endpoints;

public class OrderPaginatedRequest
    : BasePaginatedRequest
{
    public OrderPaginatedRequest(int page = 1, int size = 10)
        : base(page, size)
    {
    }
}