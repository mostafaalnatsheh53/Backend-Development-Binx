using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CardiacPatientMonitoring.Api.DTOs;

namespace CardiacPatientMonitoring.Tests;

public class HighRiskEndpointIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public HighRiskEndpointIntegrationTests(CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task PatientCannotReadAnotherPatientsMedication()
    {
        var loggedIn = await RegisterAndLoginAsync("medication-owner");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loggedIn.Token);

        var response = await client.GetAsync("/api/medications/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatientCannotReadAnotherPatientsAppointment()
    {
        var loggedIn = await RegisterAndLoginAsync("appointment-owner");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loggedIn.Token);

        var response = await client.GetAsync("/api/appointments/1");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatientCreatingInvalidVitalSignReceivesBadRequest()
    {
        var loggedIn = await RegisterAndLoginAsync("vital-sign-owner");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loggedIn.Token);

        var response = await client.PostAsJsonAsync(
            $"/api/patients/{loggedIn.PatientId}/vital-signs",
            new VitalSignRequestDto
            {
                HeartRate = 72,
                SystolicBloodPressure = 120,
                DiastolicBloodPressure = 120,
                TemperatureCelsius = 36.8m,
                RecordedAt = DateTime.UtcNow.AddMinutes(-1)
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<AuthResponseDto> RegisterAndLoginAsync(string name)
    {
        var email = $"{name}-{Guid.NewGuid():N}@example.com";
        var registration = new RegisterDto(email, "Pass123")
        {
            FirstName = "High",
            LastName = "Risk",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other"
        };

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            registration);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(email, "Pass123"));
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loggedIn = await loginResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(loggedIn);

        return loggedIn;
    }
}