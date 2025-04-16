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
        if (exception is not BusinessValidationException validationException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails()
        {
            Detail = validationException.Message,
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = validationException,
        });

        return true;
    }
}