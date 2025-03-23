using System.Collections;

using FluentValidation.TestHelper;

using MoneyGroup.WebApi.Endpoints;
using MoneyGroup.WebApi.Validators;

namespace MoneyGroup.FunctionalTests.Validators;

public class OrderPaginatedRequestValidatorTests
{
    private readonly OrderPaginatedRequestValidator _validator;

    public OrderPaginatedRequestValidatorTests()
    {
        _validator = new OrderPaginatedRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_BuyerId_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 0, participantId: 1, totalMax: 10, totalMin: 1, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BuyerId);
    }

    [Fact]
    public void Should_Have_Error_When_ParticipantId_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 0, totalMax: 10, totalMin: 1, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParticipantId);
    }

    [Fact]
    public void Should_Have_Error_When_TotalMax_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 0, totalMin: 1, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalMax);
    }

    [Fact]
    public void Should_Have_Error_When_TotalMin_Is_Less_Than_Or_Equal_To_Zero()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 10, totalMin: 0, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalMin);
    }

    [Fact]
    public void Should_Have_Error_When_TotalMin_Is_Greater_Than_TotalMax()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 5, totalMin: 10, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalMin);
    }

    [Fact]
    public void Should_Have_Error_When_Page_Is_Less_Than_One()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 10, totalMin: 1, page: 0, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Should_Have_Error_When_Size_Is_Less_Than_One()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 10, totalMin: 1, page: 1, size: 0);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Size);
    }

    [Theory]
    [MemberData(nameof(GetEnumerator))]
    public void Should_Not_Have_Error_When_Valid_Model(int? buyerId, int? participantId, decimal? totalMax, decimal? totalMin, int page, int size)
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId, participantId, totalMax, totalMin, page, size);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    public static IEnumerable<TheoryDataRow<int?, int?, decimal?, decimal?, int, int>> GetEnumerator()
    {
        yield return new TheoryDataRow<int?, int?, decimal?, decimal?, int, int>(null, null, null, null, 1, 10);
        yield return new TheoryDataRow<int?, int?, decimal?, decimal?, int, int>(1000, null, null, null, 1, 10);
        yield return new TheoryDataRow<int?, int?, decimal?, decimal?, int, int>(null, 1000, null, null, 1, 10);
        yield return new TheoryDataRow<int?, int?, decimal?, decimal?, int, int>(null, null, 2.2M, null, 1, 10);
        yield return new TheoryDataRow<int?, int?, decimal?, decimal?, int, int>(null, null, null, 1.1M, 1, 10);
        yield return new TheoryDataRow<int?, int?, decimal?, decimal?, int, int>(null, null, 2.2M, 1.1M, 1, 10);
    }
}