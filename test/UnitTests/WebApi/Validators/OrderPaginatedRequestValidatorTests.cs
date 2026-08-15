using FluentValidation.TestHelper;

using MoneyGroup.WebApi.Endpoints;
using MoneyGroup.WebApi.Validators;

namespace MoneyGroup.UnitTests.WebApi.Validators;

[Trait("Category", "Unit")]
public class OrderPaginatedRequestValidatorTests
{
    private readonly OrderPaginatedRequestValidator _validator;

    public OrderPaginatedRequestValidatorTests()
    {
        _validator = new OrderPaginatedRequestValidator(new PaginatedOptionsValidator());
    }

    [Fact]
    public void GivenRequest_WhenBuyerIdNotPositive_ThenHasErrorForBuyerId()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 0, participantId: 1, totalMax: 10, totalMin: 1, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BuyerId);
    }

    [Fact]
    public void GivenRequest_WhenParticipantIdNotPositive_ThenHasErrorForParticipantId()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 0, totalMax: 10, totalMin: 1, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ParticipantId);
    }

    [Fact]
    public void GivenRequest_WhenTotalMaxNotPositive_ThenHasErrorForTotalMax()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 0, totalMin: 1, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalMax);
    }

    [Fact]
    public void GivenRequest_WhenTotalMinNotPositive_ThenHasErrorForTotalMin()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 10, totalMin: 0, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalMin);
    }

    [Fact]
    public void GivenRequest_WhenTotalMinExceedsTotalMax_ThenHasErrorForTotalMin()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 5, totalMin: 10, page: 1, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalMin);
    }

    [Fact]
    public void GivenRequest_WhenPageBelowOne_ThenHasErrorForPage()
    {
        // Arrange
        var model = new OrderPaginatedRequest(buyerId: 1, participantId: 1, totalMax: 10, totalMin: 1, page: 0, size: 10);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void GivenRequest_WhenSizeBelowOne_ThenHasErrorForSize()
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
    public void GivenRequest_WhenValid_ThenHasNoErrors(int? buyerId, int? participantId, decimal? totalMax, decimal? totalMin, int page, int size)
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
