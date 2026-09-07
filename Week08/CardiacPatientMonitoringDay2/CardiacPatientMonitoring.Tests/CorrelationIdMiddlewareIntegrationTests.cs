using System.Net;
using System.Net.Http.Json;

namespace CardiacPatientMonitoring.Tests;

public class CorrelationIdMiddlewareIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public CorrelationIdMiddlewareIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task AnonymousAuthEndpoint_ReturnsCorrelationId()
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                Email = $"invalid-{Guid.NewGuid():N}@example.com",
                Password = "Pass123",
                FirstName = "Invalid",
                LastName = "Date",
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                Gender = "Other"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        AssertCorrelationId(response);
    }

    [Fact]
    public async Task ProtectedPatientEndpoint_ReturnsCorrelationIdWithoutAuth()
    {
        client.DefaultRequestHeaders.Authorization = null;

        var response = await client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        AssertCorrelationId(response);
    }

    [Fact]
    public async Task ExceptionEndpoint_ReturnsSameRequestedCorrelationId()
    {
        const string requestedCorrelationId = "test-correlation-id";

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/test/exception");
        request.Headers.Add("X-Correlation-ID", requestedCorrelationId);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            requestedCorrelationId,
            response.Headers.GetValues("X-Correlation-ID").Single());
    }

    private static void AssertCorrelationId(HttpResponseMessage response)
    {
        var values = response.Headers.GetValues("X-Correlation-ID").ToArray();

        Assert.Single(values);
        Assert.False(string.IsNullOrWhiteSpace(values[0]));
        Assert.DoesNotContain("\r", values[0]);
        Assert.DoesNotContain("\n", values[0]);
    }
}