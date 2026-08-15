using FluentValidation.TestHelper;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.WebApi.Validators;

namespace MoneyGroup.UnitTests.WebApi.Validators;

[Trait("Category", "Unit")]
public class OrderDtoValidatorTests
{
    private readonly OrderDtoValidator _validator = new(new ParticipantDtoValidator());

    [Fact]
    public async Task GivenOrderDto_WhenValid_ThenHasNoErrors()
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
    public async Task GivenOrderDto_WhenTitleEmpty_ThenHasErrorForTitle()
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
    public async Task GivenOrderDto_WhenDescriptionNull_ThenHasNoErrorForDescription()
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
    public async Task GivenOrderDto_WhenBuyerIdZero_ThenHasErrorForBuyerId()
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
    public async Task GivenOrderDto_WhenTotalNegative_ThenHasErrorForTotal()
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
    public async Task GivenOrderDto_WhenParticipantsNull_ThenHasErrorForParticipants()
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
    public async Task GivenOrderDto_WhenParticipantsEmpty_ThenHasErrorForParticipants()
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
    public async Task GivenOrderDto_WhenParticipantsContainsNull_ThenHasErrorForParticipants()
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
    public async Task GivenOrderDto_WhenParticipantsDuplicate_ThenHasDuplicatedParticipantError()
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
