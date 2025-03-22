using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Infrastructure.Data;
using MoneyGroup.IntegrationTests.Fixtures;

namespace MoneyGroup.IntegrationTests.Data;
public class OrderRepositoryTest
    : IClassFixture<EfRepositoryFixture>
{
    private readonly EfRepositoryFixture _dbContextFixture;
    private readonly ITestOutputHelper _output;

    public OrderRepositoryTest(EfRepositoryFixture dbContextFixture, ITestOutputHelper output)
    {
        _dbContextFixture = dbContextFixture;
        _output = output;
    }

    [Fact]
    public async Task AddOrder_ValidDto_ShouldAddedOrder()
    {
        // Arrange
        await using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _dbContextFixture.Mapper;
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
        await using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _dbContextFixture.Mapper;
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
        await repository.AddAsync(orderDto, TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();
        var updateOrderDto = new OrderDto
        {
            Id = orderDto.Id,
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
            .FirstOrDefaultAsync(o => o.Id == orderDto.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(updatedOrder);
        Assert.Equal(2, updatedOrder.BuyerId);
        Assert.Equal(updateOrderDto.Participants.Select(o => o.Id), updatedOrder.Participants.Select(c => c.ParticipantId));
    }

    [Fact]
    public async Task DeleteOrder_QueriedEntity_ShouldSuccess()
    {
        // Arrange
        using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _dbContextFixture.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var newOrder = new Order
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            BuyerId = 1,
            Participants = [
                new() { ParticipantId = 1 },
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 },
            ],
        };
        var addedOrder = await repository.AddAsync(newOrder, TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        // Act
        _output.WriteLine(addedOrder.ToJson());
        await repository.RemoveAsync(addedOrder, TestContext.Current.CancellationToken);

        // Assert
        var deletedOrder = await dbContext.Orders.FindAsync([addedOrder.Id], TestContext.Current.CancellationToken);
        Assert.Null(deletedOrder);
    }

    [Fact]
    public async Task DeleteOrder_ValidId_ShouldSuccess()
    {
        // Arrange
        using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        var mapper = _dbContextFixture.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var newOrder = new Order
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            BuyerId = 1,
            Participants = [
                new() { ParticipantId = 1 },
            ],
        };
        var addedOrder = await repository.AddAsync(newOrder, TestContext.Current.CancellationToken);
        dbContext.ChangeTracker.Clear();

        // Act
        await repository.RemoveAsync(new Order { Id = addedOrder.Id }, TestContext.Current.CancellationToken);

        // Assert
        var deletedOrder = await dbContext.Orders.FindAsync([addedOrder.Id], TestContext.Current.CancellationToken);
        Assert.Null(deletedOrder);
    }
}