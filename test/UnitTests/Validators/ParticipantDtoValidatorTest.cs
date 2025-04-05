using FluentValidation.TestHelper;

using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Validators;

namespace MoneyGroup.UnitTests.Validators;

public class ParticipantDtoValidatorTest
    : IClassFixture<ParticipantDtoValidator>
{
    private readonly ParticipantDtoValidator _validator;

    public ParticipantDtoValidatorTest(ParticipantDtoValidator validator)
    {
        _validator = validator;
    }

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