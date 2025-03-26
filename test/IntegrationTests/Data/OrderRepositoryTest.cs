using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Specifications;
using MoneyGroup.Infrastructure.Data;
using MoneyGroup.IntegrationTests.Fixtures;

namespace MoneyGroup.IntegrationTests.Data;
public class OrderRepositoryTest
    : IClassFixture<ApplicationDbContextFactory>
{
    private readonly ApplicationDbContextFactory _factory;
    private readonly ITestOutputHelper _output;

    public OrderRepositoryTest(ApplicationDbContextFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    private static Order GetSeedOrder() => new()
    {
        Title = "AddOrder",
        Description = "AddOrder",
        Total = 100.00m,
        BuyerId = 1,
        Participants = [
            new () { ParticipantId = 1 },
            new () { ParticipantId = 3 },
        ],
    };

    [Fact]
    public async Task AddOrder_ValidDto_ShouldAddedOrder()
    {
        // Arrange
        await using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _factory.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var orderDto = new OrderDto
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            BuyerId = 1,
            Participants = [
                new() { Id = 1 },
                new() { Id = 3 },
            ],
        };

        // Act
        await repository.AddAsync(orderDto, TestContext.Current.CancellationToken);
        _output.WriteLine(orderDto.ToJson());

        // Assert
        Assert.NotNull(orderDto);
        Assert.NotEqual(0, orderDto.Id);

        dbContext.ChangeTracker.Clear();
        var createdOrder = await dbContext.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Participants)
            .FirstOrDefaultAsync(o => o.Id == orderDto.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(createdOrder);
        Assert.Equal(orderDto.Id, createdOrder.Id);
        Assert.Equal(1, createdOrder.BuyerId);
        Assert.Equal(orderDto.Participants.Select(o => o.Id), createdOrder.Participants.Select(c => c.ParticipantId));
    }

    [Fact]
    public async Task UpdateOrder_ValidDto_ShouldAddedOrder()
    {
        // Arrange
        await using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _factory.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var order = GetSeedOrder();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();
        var updateOrderDto = new OrderDto
        {
            Id = order.Id,
            Title = "UpdatedOrder",
            Description = "UpdatedOrder",
            Total = 100.00m,
            BuyerId = 2,
            Participants = [
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            ],
        };

        // Act
        await repository.UpdateAsync(updateOrderDto, TestContext.Current.CancellationToken);
        _output.WriteLine(updateOrderDto.ToJson());

        // Assert
        dbContext.ChangeTracker.Clear();
        var updatedOrder = await dbContext.Orders
            .Include(o => o.Buyer)
            .Include(o => o.Participants.OrderBy(c => c.ParticipantId))
            .FirstOrDefaultAsync(o => o.Id == order.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updatedOrder);
        Assert.Equal(2, updatedOrder.BuyerId);
        Assert.Equal(updateOrderDto.Participants.Select(o => o.Id), updatedOrder.Participants.Select(c => c.ParticipantId));
    }

    [Fact]
    public async Task DeleteOrder_QueriedEntity_ShouldSuccess()
    {
        // Arrange
        using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _factory.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var order = GetSeedOrder();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        // Act
        await repository.RemoveAsync(order, TestContext.Current.CancellationToken);
        _output.WriteLine(order.ToJson());

        // Assert
        var deletedOrder = await dbContext.Orders.FindAsync([order.Id], TestContext.Current.CancellationToken);
        Assert.Null(deletedOrder);
    }

    [Fact]
    public async Task DeleteOrder_ValidId_ShouldSuccess()
    {
        // Arrange
        using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _factory.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var order = GetSeedOrder();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        // Act
        await repository.RemoveAsync(new Order { Id = order.Id }, TestContext.Current.CancellationToken);

        // Assert
        var deletedOrder = await dbContext.Orders.FindAsync([order.Id], TestContext.Current.CancellationToken);
        Assert.Null(deletedOrder);
    }

    [Fact]
    public async Task GetByPage_ValidFilter_ShouldSuccess()
    {
        // Arrange
        var buyerId = 1;
        var participantId = 1;
        var totalMax = 10_000_000M;
        var totalMin = 0M;
        int page = 1;
        int size = 10;
        var options = new OrderPaginatedOptions(buyerId, participantId, totalMax, totalMin, page, size);
        var spec = new OrderPaginatedSpec(options);
        await using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var order = new Order
        {
            Title = "NotInFilterOrder",
            BuyerId = 2,
            Participants = [
                new() { ParticipantId = 2 },
            ],
            Total = 100_000_000M,
        };
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        var mapper = _factory.Mapper;
        var repository = new OrderRepository(dbContext, mapper);

        // Act
        var model = await repository.GetByPageAsync<OrderDetailedDto>(spec, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(model);
        Assert.NotEmpty(model.Items);
        Assert.All(model.Items, o =>
        {
            Assert.NotEqual(order.Id, o.Id);
            Assert.Equal(buyerId, o.BuyerId);
            Assert.Contains(o.Participants, o => o.Id == participantId);
            Assert.InRange(o.Total, totalMin, totalMax);
        });
    }
}