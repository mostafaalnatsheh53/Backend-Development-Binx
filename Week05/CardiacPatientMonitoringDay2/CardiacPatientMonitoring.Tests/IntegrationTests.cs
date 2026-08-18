using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Api.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace CardiacPatientMonitoring.Tests;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatientById_ReturnsPatient_WhenPatientExists()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateTestJwt("user-1"));

        var response = await _client.GetAsync("/api/patients/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patient = await response.Content.ReadFromJsonAsync<PatientResponseDto>();

        Assert.NotNull(patient);
        Assert.Equal(1, patient!.Id);
        Assert.Equal("Alex", patient.FirstName);
        Assert.Equal("Taylor", patient.LastName);
    }

    [Fact]
    public async Task GetPatientById_Returns404_WhenPatientDoesNotExist()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateTestJwt("user-1"));

        var response = await _client.GetAsync("/api/patients/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPatients_Returns200_WhenRequestContainsValidJwt()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateTestJwt("user-1"));

        var response = await _client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var patients = await response.Content.ReadFromJsonAsync<List<PatientResponseDto>>();

        Assert.NotNull(patients);
        Assert.Contains(patients!, p => p.Id == 1 && p.FirstName == "Alex");
    }

    [Fact]
    public async Task GetPatients_RequiresAuthentication()
    {
        var response = await _client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_Returns201_WhenValidPayloadIsSent()
    {
        var payload = new RegisterDto("newuser@test.com", "Pass123");

        var response = await _client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static string CreateTestJwt(string userId)
    {
        var key = Encoding.UTF8.GetBytes(
            "ThisIsAVeryLongSecretKeyForJWTTokenSigningPurposesAndMustBeAtLeast32BytesLongForHS256");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, "test@cardiac.com")
        };

        var token = new JwtSecurityToken(
            issuer: "CardiacPatientMonitoring",
            audience: "CardiacPatientMonitoringClient",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
