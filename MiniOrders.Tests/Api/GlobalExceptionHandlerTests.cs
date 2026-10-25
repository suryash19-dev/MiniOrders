using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MiniOrders.Api.Middlewares;
using MiniOrders.Application.Exceptions;

namespace MiniOrders.Tests.Api;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task ValidationException_is_written_as_validation_problem_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ValidationException(new[]
        {
            new ValidationFailure("Name", "Name is required."),
            new ValidationFailure("Name", "Name is too short.")
        });

        var handled = await CreateHandler().TryHandleAsync(context, exception, CancellationToken.None);
        context.Response.Body.Position = 0;
        using var body = await JsonDocument.ParseAsync(context.Response.Body);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal("Validation failed", body.RootElement.GetProperty("title").GetString());
        Assert.Equal(2, body.RootElement.GetProperty("errors").GetProperty("Name").GetArrayLength());
    }

    [Fact]
    public async Task NotFoundException_is_written_as_404_problem_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new NotFoundException("Customer", Guid.NewGuid());

        var handled = await CreateHandler().TryHandleAsync(context, exception, CancellationToken.None);
        context.Response.Body.Position = 0;
        using var body = await JsonDocument.ParseAsync(context.Response.Body);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal("Resource not found", body.RootElement.GetProperty("title").GetString());
        Assert.Contains("Customer", body.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task Unexpected_exception_is_written_as_generic_500_problem_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var handled = await CreateHandler().TryHandleAsync(context, new InvalidOperationException("internal details"), CancellationToken.None);
        context.Response.Body.Position = 0;
        using var body = await JsonDocument.ParseAsync(context.Response.Body);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("An unexpected error occurred", body.RootElement.GetProperty("title").GetString());
        Assert.False(body.RootElement.TryGetProperty("detail", out _));
    }

    private static GlobalExceptionHandler CreateHandler() => new(NullLogger<GlobalExceptionHandler>.Instance);
}