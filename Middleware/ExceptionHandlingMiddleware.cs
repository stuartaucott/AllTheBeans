using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AllTheBeans.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException vex)
        {
            await WriteProblem(ctx, StatusCodes.Status400BadRequest,
                "Validation failed",
                string.Join("; ", vex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
        }
        catch (KeyNotFoundException nex)
        {
            await WriteProblem(ctx, StatusCodes.Status404NotFound,
                "Not found", nex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteProblem(ctx, StatusCodes.Status500InternalServerError,
                "Server error", "An unexpected error occurred.");
        }
    }

    private static Task WriteProblem(HttpContext ctx, int status, string title, string detail)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";
        return ctx.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = ctx.Request.Path
        });
    }
}