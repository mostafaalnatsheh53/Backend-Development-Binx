using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
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
        Assert.Contains(
            token.Claims,
            c => c.Type == ClaimTypes.Role && c.Value == "Patient");
        Assert.Equal("CardiacPatientMonitoring", token.Issuer);
        Assert.Contains("CardiacPatientMonitoringClient", token.Audiences);
        Assert.True(token.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task CustomerToken_IsRejectedFromAdminOnlyEndpoints()
    {
        var email = $"customer-admin-check-{Guid.NewGuid():N}@example.com";
        var register = new RegisterDto(email, "Pass123")
        {
            FirstName = "Customer",
            LastName = "User",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Female",
            PhoneNumber = "555-0103"
        };

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            register);

        Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
        var registered = await registrationResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(registered);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(email, "Pass123"));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loggedIn = await loginResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(loggedIn);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                loggedIn!.Token);

        var listPatientsResponse = await client.GetAsync("/api/patients");
        Assert.Equal(HttpStatusCode.Forbidden, listPatientsResponse.StatusCode);

        var createPatientResponse = await client.PostAsJsonAsync(
            "/api/patients",
            new { FirstName = "Blocked", LastName = "User", DateOfBirth = new DateOnly(1990, 1, 1), Gender = "Other" });

        Assert.Equal(HttpStatusCode.Forbidden, createPatientResponse.StatusCode);
    }

    [Fact]
    public async Task CustomerB_CannotFetchCustomerAResource()
    {
        var customerAEmail = $"customer-a-{Guid.NewGuid():N}@example.com";
        var customerBEmail = $"customer-b-{Guid.NewGuid():N}@example.com";

        var customerA = await RegisterAndLoginAsync(customerAEmail, "Pass123");
        var customerB = await RegisterAndLoginAsync(customerBEmail, "Pass123");

        Assert.NotNull(customerA);
        Assert.NotNull(customerB);
        Assert.NotEqual(customerA.PatientId, customerB.PatientId);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                customerB.Token);

        var response = await client.GetAsync($"/api/patients/{customerA.PatientId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var email = $"invalid-login-{Guid.NewGuid():N}@example.com";
        var registration = new RegisterDto(email, "Pass123")
        {
            FirstName = "Login",
            LastName = "Test",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other"
        };

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            registration);

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(email, "Wrong123"));

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    private async Task<AuthResponseDto?> RegisterAndLoginAsync(string email, string password)
    {
        var registration = new RegisterDto(email, password)
        {
            FirstName = email.Contains("customer-a") ? "Customer" : "Customer",
            LastName = email.Contains("customer-a") ? "A" : "B",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other",
            PhoneNumber = "555-0000"
        };

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            registration);

        Assert.Equal(HttpStatusCode.Created, registrationResponse.StatusCode);
        var registered = await registrationResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(registered);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginDto(email, password));

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loggedIn = await loginResponse.Content
            .ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(loggedIn);

        return loggedIn;
    }
}
