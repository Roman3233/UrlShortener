using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Core.Exceptions;

namespace UrlShortener.Web.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            DuplicateUrlException ex => (StatusCodes.Status409Conflict, "Duplicate URL", ex.Message),
            UserAlreadyExistsException ex => (StatusCodes.Status409Conflict, "User Already Exists", ex.Message),
            ShortUrlNotFoundException ex => (StatusCodes.Status404NotFound, "Not Found", ex.Message),
            ForbiddenDeleteException ex => (StatusCodes.Status403Forbidden, "Forbidden", ex.Message),
            ArgumentException ex => (StatusCodes.Status400BadRequest, "Bad Request", ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        // Add a friendly 'message' extension property for clients (Angular)
        problemDetails.Extensions["message"] = detail;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
