using FluentValidation.TestHelper;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.UnitTests.Validators;

public class OrderDtoValidatorTestFixture
{
    public OrderDtoValidator Validator { get; }

    public OrderDtoValidatorTestFixture()
    {
        Validator = new OrderDtoValidator(new ConsumerDtoValidator());
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
            IssuerId = 1,
            Consumers =
            [
                new ConsumerDto() { Id = 1 },
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
    public async Task GivenOrderDto_WhenIssuerIdZero_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            IssuerId = 0,
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.IssuerId);
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
    public async Task GivenOrderDto_WhenConsumersNull_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Consumers = null!,
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Consumers);
    }

    [Fact]
    public async Task GivenOrderDto_WhenConsumersEmpty_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Consumers = [],
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Consumers);
    }

    [Fact]
    public async Task GivenOrderDto_WhenConsumersContainsNull_ThenReturnError()
    {
        // Arrange
        var order = new OrderDto()
        {
            Consumers = [null!],
        };

        // Act
        var result = await _validator.TestValidateAsync(order, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(o => o.Consumers);
    }
}