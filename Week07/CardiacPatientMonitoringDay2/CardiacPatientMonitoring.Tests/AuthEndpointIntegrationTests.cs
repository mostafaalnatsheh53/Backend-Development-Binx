using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace CardiacPatientMonitoring.Tests;

public class AuthEndpointIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory factory;
    private readonly HttpClient client;

    public AuthEndpointIntegrationTests(CustomWebApplicationFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterThenLogin_CreatesLinkedPatientAndJwtClaim()
    {
        var email = $"patient-{Guid.NewGuid():N}@example.com";
        var registration = new RegisterDto(email, "Pass123")
        {
            FirstName = "Integration",
            LastName = "Patient",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other",
            PhoneNumber = "555-0102"
        };

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            registration);

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        var registered = await registerResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(registered);
        Assert.NotEmpty(registered!.Token);
        Assert.True(registered.PatientId > 0);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await db.Patients.FindAsync(registered.PatientId);

            Assert.NotNull(patient);
            Assert.False(string.IsNullOrWhiteSpace(patient!.UserId));
            Assert.NotNull(await db.Users.FindAsync(patient.UserId));
        }

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(email, "Pass123"));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loggedIn = await loginResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(loggedIn);
        Assert.NotEmpty(loggedIn!.Token);
        Assert.Equal(registered.PatientId, loggedIn.PatientId);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(loggedIn.Token);
        Assert.Equal(
            registered.PatientId.ToString(),
            token.Claims.Single(c => c.Type == "patient_id").Value);
        Assert.Equal("CardiacPatientMonitoring", token.Issuer);
        Assert.Contains("CardiacPatientMonitoringClient", token.Audiences);
        Assert.True(token.ValidTo > DateTime.UtcNow);
    }
}
