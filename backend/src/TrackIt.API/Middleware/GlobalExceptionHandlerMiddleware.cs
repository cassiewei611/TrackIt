using TrackIt.Application.Common.Exceptions;
using TrackIt.Domain.Common;

namespace TrackIt.API.Middleware;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message, Array.Empty<string>()),
            ForbiddenException ex => (StatusCodes.Status403Forbidden, ex.Message, Array.Empty<string>()),
            ValidationException ex => (StatusCodes.Status422UnprocessableEntity, "Validation failed.", ex.Errors.ToArray()),
            DomainException ex => (StatusCodes.Status400BadRequest, ex.Message, Array.Empty<string>()),
            ConflictException ex => (StatusCodes.Status409Conflict, ex.Message, Array.Empty<string>()),
            UnauthorizedException ex => (StatusCodes.Status401Unauthorized, ex.Message, Array.Empty<string>()),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", Array.Empty<string>())
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = statusCode,
            message,
            errors,
            traceId = context.TraceIdentifier
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
}
