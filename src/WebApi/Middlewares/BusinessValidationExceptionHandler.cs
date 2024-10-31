using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Exceptions;

namespace MoneyGroup.WebApi.Middlewares;

public class BusinessValidationExceptionHandler(IProblemDetailsService problemDetailsService)
        : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BussinessValidationException validationException)
        {
            return false;
        }

        var validationProblemDetails = new ValidationProblemDetails()
        {
            Detail = validationException.Message,
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = validationProblemDetails,
            Exception = validationException,
        });

        return true;
    }
}