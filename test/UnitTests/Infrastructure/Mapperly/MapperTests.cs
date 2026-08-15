using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Infrastructure.Mapperly;
using MoneyGroup.UnitTests.Builders;

namespace MoneyGroup.UnitTests.Infrastructure.Mapperly;

[Trait("Category", "Unit")]
public class MapperTests
{
    private readonly Mapper _mapper = new();

    [Fact]
    public void GivenParticipantDto_WhenMapped_ThenCopiesParticipantId()
    {
        // Arrange
        var dto = new ParticipantDto { ParticipantId = 42 };

        // Act
        var entity = _mapper.Map(dto);

        // Assert
        Assert.Equal(42, entity.ParticipantId);
    }

    [Fact]
    public void GivenOrderDto_WhenMapped_ThenCopiesEveryScalarField()
    {
        // Arrange
        var dto = OrderDtoBuilder.Valid()
            .WithId(7)
            .WithTitle("Dinner")
            .WithDescription("Team dinner")
            .WithTotal(1234.56m)
            .WithBuyer(3)
            .WithParticipants(1, 2)
            .Build();

        // Act
        var entity = _mapper.Map(dto);

        // Assert
        Assert.Equal(7, entity.Id);
        Assert.Equal("Dinner", entity.Title);
        Assert.Equal("Team dinner", entity.Description);
        Assert.Equal(1234.56m, entity.Total);
        Assert.Equal(3, entity.BuyerId);
        Assert.Equal([1, 2], entity.Participants.Select(p => p.ParticipantId));
    }

    [Fact]
    public void GivenOrderDto_WhenMapped_ThenLeavesBuyerNavigationUnset()
    {
        // Arrange
        var dto = OrderDtoBuilder.Valid().WithBuyer(3).Build();

        // Act
        var entity = _mapper.Map(dto);

        // Assert
        Assert.Null(entity.Buyer);
    }

    [Fact]
    public void GivenOrderParticipant_WhenMapped_ThenFlattensParticipantName()
    {
        // Arrange
        var entity = new OrderParticipant
        {
            ParticipantId = 5,
            Participant = new User { Id = 5, Name = "Manh" },
        };

        // Act
        var dto = _mapper.Map(entity);

        // Assert
        Assert.Equal(5, dto.ParticipantId);
        Assert.Equal("Manh", dto.ParticipantName);
    }

    [Fact]
    public void GivenOrder_WhenMapped_ThenFlattensBuyerName()
    {
        // Arrange
        var entity = new Order
        {
            Id = 1,
            Title = "Order 1",
            Description = "desc",
            Total = 10_000m,
            BuyerId = 1,
            Buyer = new User { Id = 1, Name = "Truong" },
            Participants =
            [
                new() { ParticipantId = 2, Participant = new User { Id = 2, Name = "Duc" } },
            ],
        };

        // Act
        var dto = _mapper.Map(entity);

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Order 1", dto.Title);
        Assert.Equal(10_000m, dto.Total);
        Assert.Equal("Truong", dto.BuyerName);
        var participant = Assert.Single(dto.Participants);
        Assert.Equal(2, participant.ParticipantId);
        Assert.Equal("Duc", participant.ParticipantName);
    }

    [Fact]
    public void GivenUserQueryable_WhenProjected_ThenMapsIdNameAndEmail()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Name = "Truong", Email = "t@d.com" },
            new() { Id = 2, Name = "Duc", Email = null },
        }.AsQueryable();

        // Act
        var projected = _mapper.Project(users).ToList();

        // Assert
        Assert.Equal(2, projected.Count);
        Assert.Equal([1, 2], projected.Select(u => u.Id));
        Assert.Equal(["Truong", "Duc"], projected.Select(u => u.Name));
        Assert.Equal("t@d.com", projected[0].Email);
        Assert.Null(projected[1].Email);
    }
}
