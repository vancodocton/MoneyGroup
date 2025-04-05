using Microsoft.EntityFrameworkCore;

using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Specifications;
using MoneyGroup.Infrastructure.Data;
using MoneyGroup.IntegrationTests.Fixtures;

namespace MoneyGroup.IntegrationTests.Data;

public class EfRepositoryTest
    : IClassFixture<SQLiteDbContextFactory>
{
    private readonly SQLiteDbContextFactory _factory;

    public EfRepositoryTest(SQLiteDbContextFactory factory)
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

    private async Task<PaginatedModel<SimpleEntity>> ActAsync(int total, int page, int size)
    {
        // Arrange
        var spec = new BasePaginatedSpecification<SimpleEntity>(new PaginatedOptions(page, size));
        await using var dbContext = _factory.CreateDbContext();
        await dbContext.Database.BeginTransactionAsync(TestContext.Current.CancellationToken);
        await dbContext.SimpleEntities.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        for (int i = 0; i < total; i++) // Seed 10 entity
        {
            var entity = new SimpleEntity
            {
                Name = $"SeedEntity{i}",
            };
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
            dbContext.ChangeTracker.Clear();
        }
        var mapper = _factory.Mapper;
        var repository = new EfRepository<SimpleEntity>(dbContext, mapper);

        // Act
        return await repository.GetByPageAsync(spec, TestContext.Current.CancellationToken);
    }
}