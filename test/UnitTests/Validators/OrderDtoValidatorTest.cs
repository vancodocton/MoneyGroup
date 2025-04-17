using FluentValidation.TestHelper;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.UnitTests.Validators;

public class OrderDtoValidatorTestFixture
{
    public OrderDtoValidator Validator { get; }

    public OrderDtoValidatorTestFixture()
    {
        Validator = new OrderDtoValidator(new ParticipantDtoValidator());
    }
}

public class OrderDtoValidatorTest
    : IClassFixture<OrderDtoValidatorTestFixture>
{
    private readonly OrderDtoValidator _validator;

    public OrderDtoValidatorTest(OrderDtoValidatorTestFixture fixture)
    {
        _validator = fixture.Validator;
    }

    [Fact]
    public async Task GivenOrderDto_WhenValid_ThenReturnNoError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Title = "Title",
            BuyerId = 1,
            Participants =
            [
                new ParticipantDto() { ParticipantId = 1 },
            ],
        };

        // Act
        var result = await _validator.ValidateAsync(order, TestContext.Current.CancellationToken);

        // Assert
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GivenOrderDto_WhenTitleEmpty_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Title = "   ",
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Title);
    }

    [Fact]
    public async Task GivenOrderDto_WhenDescriptionNull_ThenReturnNoError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Description = null,
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotHaveValidationErrorFor(o => o.Description);
    }

    [Fact]
    public async Task GivenOrderDto_WhenBuyerIdZero_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            BuyerId = 0,
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.BuyerId);
    }

    [Fact]
    public async Task GivenOrderDto_WhenTotalNegative_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Total = -1,
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Total);
    }

    [Fact]
    public async Task GivenOrderDto_WhenParticipantsNull_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Participants = null!,
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Participants);
    }

    [Fact]
    public async Task GivenOrderDto_WhenParticipantsEmpty_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Participants = [],
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Participants);
    }

    [Fact]
    public async Task GivenOrderDto_WhenParticipantsContainsNull_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Participants = [null!, null!],
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Participants);
    }

    [Fact]
    public async Task GivenOrderDto_WhenParticipantsDuplicate_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Participants = [
                new ParticipantDto() { ParticipantId = 1 },
                new ParticipantDto() { ParticipantId = 1 },
                ],
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Participants).WithErrorMessage("Duplicated participant");
    }
}
