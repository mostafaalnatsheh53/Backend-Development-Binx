namespace CardiacPatientMonitoring.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetCorrelationId(context.Request.Headers[HeaderName]);

        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await next(context);
    }

    private static string GetCorrelationId(string? requestedId)
    {
        if (!string.IsNullOrWhiteSpace(requestedId) &&
            requestedId.Length <= 128 &&
            requestedId.IndexOfAny(['\r', '\n']) < 0)
        {
            return requestedId;
        }

        return Guid.NewGuid().ToString("N");
    }
}