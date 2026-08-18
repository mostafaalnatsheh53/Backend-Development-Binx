# Hands-On Lab: Write Integration Tests

## Objective

In this lab, you will learn how to test ASP.NET Core APIs as real HTTP clients using `WebApplicationFactory<TEntryPoint>`.

The goal is to move beyond unit tests and validate that the full application pipeline works correctly:

- routing
- middleware
- authentication
- dependency injection
- database access
- JSON serialization

---

## 1. What is WebApplicationFactory?

`WebApplicationFactory<TEntryPoint>` creates the application in memory and gives you an `HttpClient` that can call the API without running a real network server.

This lets you test the application the same way a client would experience it, but in an isolated test environment.

### Why this matters

Unit tests validate individual classes, but integration tests validate the real end-to-end behavior of the API.

For example, an endpoint can fail because of:

- wrong route configuration
- middleware ordering issues
- incorrect status codes
- serialization problems
- broken authorization

These problems often do not appear in controller-only tests.

---

## 2. Setup

Add the testing package:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.10" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.10" />
</ItemGroup>
```

Then create a custom factory:

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CardiacPatientMonitoring.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
```

---

## 3. Use a Test Database

You should not use the same database as your development environment.

Use one of these approaches:

### Option A: Use EF Core InMemory

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsEnvironment("Testing"))
    {
        options.UseInMemoryDatabase("CardiacPatientMonitoringTestDb");
        return;
    }

    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

This keeps tests isolated and reproducible.

---

## 4. Write an Integration Test for the Happy Path

Test a real API request to the main resource.

Example:

```csharp
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
```

This verifies:

- route works
- controller executes
- service returns data
- JSON body is deserialized correctly
- correct HTTP status is returned

---

## 5. Write an Integration Test for the Not-Found Path

```csharp
[Fact]
public async Task GetPatientById_Returns404_WhenPatientDoesNotExist()
{
    _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", CreateTestJwt("user-1"));

    var response = await _client.GetAsync("/api/patients/99999");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}
```

This ensures the application handles missing data correctly and returns the expected HTTP response.

---

## 6. Test a Protected Endpoint with a Valid JWT

If an endpoint is protected with `[Authorize]`, the request must include a valid JWT.

```csharp
[Fact]
public async Task GetPatients_Returns200_WhenRequestContainsValidJwt()
{
    _client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", CreateTestJwt("user-1"));

    var response = await _client.GetAsync("/api/patients");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

The JWT can be generated in the test using the same signing settings used by the application:

```csharp
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
```

---

## 7. Example Test Class

```csharp
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
```

---

## 8. Expected Result

When you run the project tests:

```bash
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj --nologo
```

The test suite should pass and confirm that:

- API runs in a test host
- API routes respond correctly
- 404 handling works
- protected endpoints accept valid JWT tokens
- database is isolated from the development one

---

## 9. Summary

This lab teaches the difference between:

- Unit tests: test classes in isolation
- Integration tests: test the full app pipeline via HTTP requests

Using `WebApplicationFactory` and a test database gives you stable, realistic, and repeatable API tests.

---

## 10. Learning Outcomes

By the end of this lab, you should be able to:

- set up `WebApplicationFactory`
- write integration tests for API endpoints
- assert on actual HTTP responses
- test not-found behavior
- use a test database or in-memory provider
- attach a valid JWT to protected endpoints

