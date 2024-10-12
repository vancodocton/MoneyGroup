using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Infrastucture.Data;
using MoneyGroup.IntegrationTests.Fixtures;

using Xunit.Abstractions;

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
        await dbContext.Database.BeginTransactionAsync();
        var mapper = _dbContextFixture.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var orderDto = new OrderDto
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            IssuerId = 1,
            Consumers = [
                new() { Id = 1 },
                new() { Id = 3 },
            ],
        };

        // Act
        await repository.AddAsync(orderDto);
        _output.WriteLine(orderDto.ToJson());

        // Assert
        Assert.NotNull(orderDto);
        Assert.NotEqual(0, orderDto.Id);

        dbContext.ChangeTracker.Clear();
        var createdOrder = await dbContext.Orders
            .Include(o => o.Issuer)
            .Include(o => o.Consumers)
            .FirstOrDefaultAsync(o => o.Id == orderDto.Id);
        Assert.NotNull(createdOrder);
        Assert.Equal(orderDto.Id, createdOrder.Id);
        Assert.Equal(1, createdOrder.IssuerId);
        Assert.Equal(orderDto.Consumers.Select(o => o.Id), createdOrder.Consumers.Select(c => c.ConsumerId));
    }

    [Fact]
    public async Task UpdateOrder_ValidDto_ShouldAddedOrder()
    {
        // Arrange
        await using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync();
        var mapper = _dbContextFixture.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var orderDto = new OrderDto
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            IssuerId = 1,
            Consumers = [
                new() { Id = 1 },
                new() { Id = 3 },
            ],
        };
        await repository.AddAsync(orderDto);
        dbContext.ChangeTracker.Clear();
        var updateOrderDto = new OrderDto
        {
            Id = orderDto.Id,
            Title = "UpdatedOrder",
            Description = "UpdatedOrder",
            Total = 100.00m,
            IssuerId = 2,
            Consumers = [
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            ],
        };

        // Act
        await repository.UpdateAsync(updateOrderDto);
        _output.WriteLine(updateOrderDto.ToJson());

        // Assert
        dbContext.ChangeTracker.Clear();
        var updatedOrder = await dbContext.Orders
            .Include(o => o.Issuer)
            .Include(o => o.Consumers.OrderBy(c => c.ConsumerId))
            .FirstOrDefaultAsync(o => o.Id == orderDto.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(2, updatedOrder.IssuerId);
        Assert.Equal(updateOrderDto.Consumers.Select(o => o.Id), updatedOrder.Consumers.Select(c => c.ConsumerId));
    }

    [Fact]
    public async Task DeleteOrder_QueriedEntity_ShouldSuccess()
    {
        // Arrange
        using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync();
        var mapper = _dbContextFixture.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var newOrder = new Order
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            IssuerId = 1,
            Consumers = [
                new() { ConsumerId = 1 },
                new() { ConsumerId = 2 },
                new() { ConsumerId = 3 },
            ],
        };
        var addedOrder = await repository.AddAsync(newOrder);
        dbContext.ChangeTracker.Clear();

        // Act
        _output.WriteLine(addedOrder.ToJson());
        await repository.RemoveAsync(addedOrder);

        // Assert
        var deletedOrder = await dbContext.Orders.FindAsync(addedOrder.Id);
        Assert.Null(deletedOrder);
    }

    [Fact]
    public async Task DeleteOrder_ValidId_ShouldSuccess()
    {
        // Arrange
        using var dbContext = _dbContextFixture.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync();
        var mapper = _dbContextFixture.Mapper;
        var repository = new OrderRepository(dbContext, mapper);
        var newOrder = new Order
        {
            Title = "AddOrder",
            Description = "AddOrder",
            Total = 100.00m,
            IssuerId = 1,
            Consumers = [
                new() { ConsumerId = 1 },
            ],
        };
        var addedOrder = await repository.AddAsync(newOrder);
        dbContext.ChangeTracker.Clear();

        // Act
        await repository.RemoveAsync(new Order { Id = addedOrder.Id });

        // Assert
        var deletedOrder = await dbContext.Orders.FindAsync(addedOrder.Id);
        Assert.Null(deletedOrder);
    }
}