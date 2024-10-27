using FluentValidation.TestHelper;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.UnitTests.Validators;

public class ConsumerDtoValidatorTest
    : IClassFixture<ConsumerDtoValidator>
{
    private readonly ConsumerDtoValidator _validator;

    public ConsumerDtoValidatorTest(ConsumerDtoValidator validator)
    {
        _validator = validator;
    }

    [Fact]
    public async Task GivenOrderDto_WhenConsumerIdZero_ThenReturnError()
    {
        // Arrange
        var consumer = new ConsumerDto()
        {
            Id = 0,
        };

        // Act
        var result = await _validator.TestValidateAsync(consumer);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}