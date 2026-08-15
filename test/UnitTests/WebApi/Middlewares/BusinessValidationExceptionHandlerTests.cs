using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Exceptions;
using MoneyGroup.WebApi.Middlewares;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Middlewares;

[Trait("Category", "Unit")]
public class BusinessValidationExceptionHandlerTests
{
    private readonly IProblemDetailsService _problemDetailsService = Substitute.For<IProblemDetailsService>();
    private readonly BusinessValidationExceptionHandler _handler;

    public BusinessValidationExceptionHandlerTests()
    {
        _handler = new BusinessValidationExceptionHandler(_problemDetailsService);
    }

    [Fact]
    public async Task GivenUnrelatedException_WhenHandled_ThenReturnsFalseAndWritesNothing()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            httpContext, new InvalidOperationException("boom"), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(handled);
        await _problemDetailsService.DidNotReceive().WriteAsync(Arg.Any<ProblemDetailsContext>());
    }

    [Fact]
    public async Task GivenBuyerNotFound_WhenHandled_ThenReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            httpContext, new BuyerNotFoundException(), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(handled);
    }

    [Fact]
    public async Task GivenBuyerNotFound_WhenHandled_ThenSetsBadRequestStatus()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        await _handler.TryHandleAsync(
            httpContext, new BuyerNotFoundException(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task GivenParticipantNotFound_WhenHandled_ThenWritesProblemDetailsCarryingTheMessage()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new ParticipantNotFoundException();

        // Act
        await _handler.TryHandleAsync(httpContext, exception, TestContext.Current.CancellationToken);

        // Assert
        await _problemDetailsService.Received(1).WriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx =>
                ctx.HttpContext == httpContext
                && ctx.Exception == exception
                && ctx.ProblemDetails.Detail == "Participant not found"));
    }
}
