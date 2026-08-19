using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CardiacPatientMonitoring.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException)
        {
            await WriteProblemDetails(
                context,
                HttpStatusCode.NotFound,
                "Resource not found.");
        }
        catch (ArgumentException)
        {
            await WriteProblemDetails(
                context,
                HttpStatusCode.BadRequest,
                "The request is invalid.");
        }
        catch (UnauthorizedAccessException)
        {
            await WriteProblemDetails(
                context,
                HttpStatusCode.Unauthorized,
                "Authentication is required.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception for {HttpMethod} {RequestPath}",
                context.Request.Method,
                context.Request.Path);

            await WriteProblemDetails(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        HttpStatusCode statusCode,
        string title)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{(int)statusCode}"
        };

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problemDetails);
    }
}