using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UrlShortener.Core.Exceptions;
using UrlShortener.Web.Middleware;

namespace UrlShortener.Tests;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _logger = new();

    private GlobalExceptionHandler CreateSut() => new(_logger.Object);

    [Fact]
    public async Task TryHandleAsync_DuplicateUrlException_Returns409Conflict()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new DuplicateUrlException("https://google.com");

        var handled = await CreateSut().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status409Conflict, problem.Status);
        Assert.Equal("Duplicate URL", problem.Title);
        Assert.Contains("https://google.com", problem.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_UserAlreadyExistsException_Returns409Conflict()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new UserAlreadyExistsException("admin");

        var handled = await CreateSut().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ShortUrlNotFoundException_Returns404NotFound()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ShortUrlNotFoundException(99);

        var handled = await CreateSut().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ForbiddenDeleteException_Returns403Forbidden()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ForbiddenDeleteException();

        var handled = await CreateSut().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ArgumentException_Returns400BadRequest()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ArgumentException("Invalid argument value");

        var handled = await CreateSut().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_Returns500InternalServerError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("Unexpected system failure");

        var handled = await CreateSut().TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }
}
