using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Services;
using MoneyGroup.Infrastructure.Data;
using MoneyGroup.IntegrationTests.Fixtures;

namespace MoneyGroup.IntegrationTests.Data;

public class EfRepositoryTest
    : IClassFixture<ApplicationDbContextFactory>
{
    private readonly ApplicationDbContextFactory _factory;

    public EfRepositoryTest(ApplicationDbContextFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task WhenNotLastPage_ThenItemsCountEqualPageSize()
    {
        // Arrange
        int page = 1;
        int size = 3;
        int total = 5;

        // Act
        var model = await ActAsync(total, page, size);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(page, model.Page);
        Assert.Equal(total, model.Total);
        Assert.Equal(size, model.Count);
    }

    [Fact]
    public async Task WhenLastPage_ThenItemsCountLessThanPageSize()
    {
        // Arrange
        int page = 2;
        int size = 3;
        int total = 5;

        // Act
        var model = await ActAsync(total, page, size);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(page, model.Page);
        Assert.Equal(total, model.Total);
        Assert.True(model.Count < size);
        Assert.Equal(2, model.Count); // total - (page - 1) * size
    }

    private async Task<PaginatedModel<Order>> ActAsync(int total, int page, int size)
    {
        // Arrange
        var spec = new OrderPaginatedSpec(new PaginatedOptions(page, size));
        await using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        await dbContext.Orders.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        for (int i = 0; i < total; i++) // Seed 10 order
        {
            var orderDto = new Order
            {
                Title = "SeedOrder",
                Description = $"SeedOrder{i}",
                Total = 100.00m,
                BuyerId = 1,
                Participants = [
                    new() { ParticipantId = 1 },
                    new() { ParticipantId = 3 },
                ],
            };
            dbContext.Add(orderDto);
            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
            dbContext.ChangeTracker.Clear();
        }
        var mapper = _factory.Mapper;
        var repository = new OrderRepository(dbContext, mapper);

        // Act
        return await repository.GetByPageAsync(spec, TestContext.Current.CancellationToken);
    }
}