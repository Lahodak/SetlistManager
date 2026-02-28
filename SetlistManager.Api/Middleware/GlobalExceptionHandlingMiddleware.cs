using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Exceptions;

namespace SetlistManager.Api.Middleware;

public class GlobalExceptionHandlingMiddleware(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandlingMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");

        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            EntryNotFoundException => StatusCodes.Status404NotFound,
            GeniusAccessTokenNotRecievedException => StatusCodes.Status502BadGateway,
            DuplicateEntryException => StatusCodes.Status409Conflict,
            InvalidOperationException => StatusCodes.Status400BadRequest,            
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occured",
                Detail = exception.Message
            }
        });
    }
}