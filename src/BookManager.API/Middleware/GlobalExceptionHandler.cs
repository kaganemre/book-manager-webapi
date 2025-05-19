using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManager.API.Middleware;

public class GlobalExceptionHandler(
    IHostEnvironment env,
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            DbUpdateException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = env.IsDevelopment() ? exception.Message : "Bir hata oluştu! Lütfen tekrar deneyin.",
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = env.IsDevelopment() ? exception.StackTrace : null,
        };

        logger.LogError(exception, """
        Beklenmeyen bir hata oluştu.
        Message: {Message}
        Inner: {Inner}
        StackTrace: {StackTrace}
        """,
        exception.Message,
        exception.InnerException?.Message,
        exception.StackTrace);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}