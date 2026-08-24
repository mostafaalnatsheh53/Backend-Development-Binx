using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Api.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace CardiacPatientMonitoring.Tests;

public class PatientEndpointIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public PatientEndpointIntegrationTests(CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatients_WithValidJwt_ReturnsSeededPatients()
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateTestJwt());

        var response = await client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var patients = await response.Content
            .ReadFromJsonAsync<List<PatientResponseDto>>();

        Assert.NotNull(patients);
        Assert.Contains(
            patients!,
            patient => patient.Id == 1 && patient.FirstName == "Alex");
    }

    [Fact]
    public async Task GetPatients_WithoutJwt_ReturnsUnauthorized()
    {
        client.DefaultRequestHeaders.Authorization = null;

        var response = await client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static string CreateTestJwt()
    {
        var key = Encoding.UTF8.GetBytes(
            "ThisIsAVeryLongSecretKeyForJWTTokenSigningPurposesAndMustBeAtLeast32BytesLongForHS256");
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "integration-user"),
            new Claim(ClaimTypes.Email, "integration@example.com")
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
