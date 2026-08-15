using FluentValidation.TestHelper;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.WebApi.Validators;

namespace MoneyGroup.UnitTests.WebApi.Validators;

[Trait("Category", "Unit")]
public class ParticipantDtoValidatorTests
{
    private readonly ParticipantDtoValidator _validator = new();

    [Fact]
    public async Task GivenOrderDto_WhenParticipantIdZero_ThenReturnError()
    {
        // Arrange
        var participant = new ParticipantDto()
        {
            ParticipantId = 0,
        };


        // Act
        var result = await _validator.TestValidateAsync(participant, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.ParticipantId);
    }
}
