using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MoneyGroup.WebApi.Middlewares;

public class FluentValidationExceptionHandler(IProblemDetailsService problemDetailsService)
        : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());

        var validationProblemDetails = new ValidationProblemDetails(errors);

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