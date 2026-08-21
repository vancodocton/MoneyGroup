using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Core.Specifications;

namespace MoneyGroup.UnitTests.Core.Specifications;

[Trait("Category", "Unit")]
public class SpecificationTests
{
    private static Order Order(int id, int buyerId = 1, decimal total = 100, params int[] participantIds) => new()
    {
        Id = id,
        Title = $"Order {id}",
        BuyerId = buyerId,
        Total = total,
        Participants = [.. participantIds.Select(p => new OrderParticipant { ParticipantId = p, OrderId = id })],
    };

    [Fact]
    public void GivenEntityByIdSpec_WhenIdMatches_ThenIsSatisfied()
    {
        var spec = new EntityByIdSpec<Order>(5);

        Assert.True(spec.IsSatisfiedBy(Order(5)));
        Assert.False(spec.IsSatisfiedBy(Order(6)));
    }

    [Fact]
    public void GivenEntityByIdsSpec_WhenIdInSet_ThenIsSatisfied()
    {
        var spec = new EntityByIdsSpec<User>([1, 3]);

        Assert.True(spec.IsSatisfiedBy(new User { Id = 1, Name = "A" }));
        Assert.True(spec.IsSatisfiedBy(new User { Id = 3, Name = "C" }));
        Assert.False(spec.IsSatisfiedBy(new User { Id = 2, Name = "B" }));
    }

    [Fact]
    public void GivenEntityByIdsSpec_WhenIdsEmpty_ThenNothingIsSatisfied()
    {
        var spec = new EntityByIdsSpec<User>([]);

        Assert.False(spec.IsSatisfiedBy(new User { Id = 1, Name = "A" }));
    }

    [Fact]
    public void GivenUserByEmailSpec_WhenEmailMatches_ThenIsSatisfied()
    {
        var spec = new UserByEmailSpec("a@b.com");

        Assert.True(spec.IsSatisfiedBy(new User { Id = 1, Name = "A", Email = "a@b.com" }));
        Assert.False(spec.IsSatisfiedBy(new User { Id = 2, Name = "B", Email = "other@b.com" }));
        Assert.False(spec.IsSatisfiedBy(new User { Id = 3, Name = "C", Email = null }));
    }

    [Fact]
    public void GivenBasePaginatedSpec_WhenFirstPage_ThenSkipsNoneAndTakesPageSize()
    {
        var spec = new BasePaginatedSpecification<User>(new PaginatedOptions(page: 1, size: 3));

        Assert.Equal(0, spec.Skip);
        Assert.Equal(3, spec.Take);
    }

    [Fact]
    public void GivenBasePaginatedSpec_WhenThirdPage_ThenSkipsTwoPages()
    {
        var spec = new BasePaginatedSpecification<User>(new PaginatedOptions(page: 3, size: 10));

        Assert.Equal(20, spec.Skip);
        Assert.Equal(10, spec.Take);
    }

    [Fact]
    public void GivenBasePaginatedSpec_WhenEvaluated_ThenReturnsRequestedWindow()
    {
        var users = Enumerable.Range(1, 10)
            .Select(i => new User { Id = i, Name = $"U{i}" })
            .ToList();
        var spec = new BasePaginatedSpecification<User>(new PaginatedOptions(page: 2, size: 3));

        var result = spec.Evaluate(users).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal([4, 5, 6], result.Select(u => u.Id));
    }

    [Fact]
    public void GivenUserPaginatedSpec_WhenKeywordSupplied_ThenFiltersByNameContains()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Truong" },
            new() { Id = 2, Name = "Duc" },
            new() { Id = 3, Name = "Manh" },
        };
        var spec = new UserPaginatedSpec(new UserPaginatedOptions(1, 10) { Keyword = "ruo" });

        var result = spec.Evaluate(users).ToList();

        Assert.Equal([1], result.Select(u => u.Id));
    }

    [Fact]
    public void GivenUserPaginatedSpec_WhenKeywordBlank_ThenReturnsEveryone()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Truong" },
            new() { Id = 2, Name = "Duc" },
        };
        var spec = new UserPaginatedSpec(new UserPaginatedOptions(1, 10) { Keyword = "   " });

        var result = spec.Evaluate(users).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenNoFilters_ThenOrdersByIdDescending()
    {
        var orders = new List<Order> { Order(1), Order(3), Order(2) };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(null, null, null, null, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([3, 2, 1], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenBuyerIdSupplied_ThenFiltersByBuyer()
    {
        var orders = new List<Order> { Order(1, buyerId: 1), Order(2, buyerId: 2) };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(2, null, null, null, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([2], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenParticipantIdSupplied_ThenFiltersByParticipant()
    {
        var orders = new List<Order>
        {
            Order(1, participantIds: [1, 2]),
            Order(2, participantIds: [3]),
        };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(null, 3, null, null, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([2], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenTotalBoundsSupplied_ThenFiltersInclusively()
    {
        var orders = new List<Order>
        {
            Order(1, total: 50),
            Order(2, total: 100),
            Order(3, total: 150),
        };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(null, null, 150m, 100m, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([3, 2], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenAllFiltersCombined_ThenAppliesEveryFilter()
    {
        var orders = new List<Order>
        {
            Order(1, buyerId: 1, total: 100, participantIds: [1]),
            Order(2, buyerId: 1, total: 500, participantIds: [1]),
            Order(3, buyerId: 2, total: 100, participantIds: [1]),
            Order(4, buyerId: 1, total: 100, participantIds: [9]),
        };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(1, 1, 200m, 50m, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([1], result.Select(o => o.Id));
    }
}
