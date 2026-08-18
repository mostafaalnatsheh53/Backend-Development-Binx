using System.Net;
using System.Text.Json;
using CardiacPatientMonitoring.Api.Services;

namespace CardiacPatientMonitoring.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext c)
    {
        try
        {
            await next(c);
        }
        catch (NotFoundException e)
        {
            await Write(c, HttpStatusCode.NotFound, e.Message);
        }
        catch (ArgumentException e)
        {
            await Write(c, HttpStatusCode.BadRequest, e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            await Write(c, HttpStatusCode.Unauthorized, e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception");

            await Write(
                c,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task Write(
        HttpContext c,
        HttpStatusCode s,
        string m)
    {
        c.Response.StatusCode = (int)s;
        c.Response.ContentType = "application/json";

        await c.Response.WriteAsync(
            JsonSerializer.Serialize(new
            {
                status = (int)s,
                message = m
            }));
    }
}