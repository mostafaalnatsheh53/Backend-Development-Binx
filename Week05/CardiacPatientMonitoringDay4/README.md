# Day 4 - Global Exception Handling

## Overview

Day 4 improves the existing CardiacPatientMonitoring ASP.NET Core Web API by adding centralized error handling, safe API error responses, structured logging, and automated verification.

The API is a healthcare management backend with endpoints for:

- User registration and login
- Patient management
- Vital signs
- Medications
- Appointments

The project uses ASP.NET Core Web API, Entity Framework Core with SQL Server, ASP.NET Core Identity, JWT authentication, Swagger, and xUnit tests.

## Objective

Centralized exception handling prevents every controller and service from repeating the same `try/catch` logic. Controllers and services can allow unexpected exceptions to bubble up, while one middleware handles the response and logging consistently.

## Project Structure

```text
CardiacPatientMonitoringDay4/
|-- CardiacPatientMonitoring.Api/
|   |-- Controllers/
|   |-- Data/
|   |-- DTOs/
|   |-- Middleware/
|   |-- Models/
|   |-- Services/
|   |-- Program.cs
|   `-- CardiacPatientMonitoring.Api.csproj
|-- CardiacPatientMonitoring.Tests/
|   |-- ControllerAndMiddlewareTests.cs
|   |-- PatientsControllerTests.cs
|   |-- RiskScoreCalculatorTests.cs
|   `-- CardiacPatientMonitoring.Tests.csproj
|-- CardiacPatientMonitoring.slnx
`-- README.md
```

## What Was Implemented

### 1. Global Exception-Handling Middleware

`ExceptionHandlingMiddleware` is registered early in `Program.cs`, before authentication, authorization, and controller endpoints. It catches exceptions thrown by downstream middleware, controllers, and services.

Known application exceptions are mapped to safe HTTP responses:

| Exception | Status | Title |
| --- | ---: | --- |
| `NotFoundException` | 404 | `Resource not found.` |
| `ArgumentException` | 400 | `The request is invalid.` |
| `UnauthorizedAccessException` | 401 | `Authentication is required.` |
| Other exceptions | 500 | `An unexpected error occurred.` |

### 2. ProblemDetails Responses

The API uses the standard ASP.NET Core `ProblemDetails` model and `application/problem+json` content type.

Example unexpected-error response:

```json
{
    "type": "https://httpstatuses.com/500",
    "title": "An unexpected error occurred.",
    "status": 500
}
```

The response does not include:

- `Exception.Message`
- Stack traces
- File paths
- Database details
- Connection strings
- Internal implementation details

The complete exception is available only in server-side logs.

### 3. Structured Logging

Unexpected exceptions are logged with `ILogger` and structured placeholders:

```csharp
logger.LogError(
        exception,
        "Unhandled exception for {HttpMethod} {RequestPath}",
        context.Request.Method,
        context.Request.Path);
```

This keeps the exception, HTTP method, and request path searchable as separate logging properties. No string interpolation is used.

### 4. Test Exception Endpoint

The isolated test controller provides a deliberate failure for lab verification:

```text
GET /api/test/exception
```

It throws a controlled `InvalidOperationException`. The endpoint is marked anonymous and is intentionally kept under `api/test` so it is easy to identify as a test-only endpoint.

### 5. Redundant Try/Catch Review

The existing controllers and services do not contain `try/catch` blocks that only log an exception and return a generic 500 response. No business logic or meaningful exception handling was removed.

## Running the API

From the repository root:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project .\Week05\CardiacPatientMonitoringDay4\CardiacPatientMonitoring.Api\CardiacPatientMonitoring.Api.csproj --urls http://localhost:5099
```

When running in Development mode:

- Swagger UI: `http://localhost:5099/swagger/index.html`
- OpenAPI document: `http://localhost:5099/openapi/v1.json`

The API also requires the configured SQL Server connection string and JWT settings from the application configuration.

## Testing

Run all tests with:

```powershell
dotnet test .\Week05\CardiacPatientMonitoringDay4\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo
```

Build the complete solution with:

```powershell
dotnet build .\Week05\CardiacPatientMonitoringDay4\CardiacPatientMonitoring.slnx --nologo
```

### Verified Normal Endpoint

```text
GET /swagger/index.html
Expected: 200 OK
```

### Verified Exception Endpoint

```text
GET /api/test/exception
Expected: 500 Internal Server Error
Content-Type: application/problem+json
```

The response was verified to contain the safe ProblemDetails payload only. The controlled exception message and stack trace were not returned. The server console contained the exception, stack trace, HTTP method, and request path.

## Test Coverage

The xUnit test project covers:

- Controller success responses
- Controller exception propagation
- Patient operations
- Middleware mapping for known exceptions
- Middleware safety for unexpected exceptions
- Risk score calculation rules

The middleware safety test specifically confirms that an exception message, exception type, and stack trace are not present in the HTTP response.

## Verification Result

- Solution build: successful
- Test project: successful
- Tests: `18 passed, 0 failed`
- Swagger smoke test: `200 OK`
- Exception endpoint smoke test: `500 ProblemDetails`
- Internal exception details: logged server-side only

## Decisions and Limitations

- No new NuGet packages were added.
- The existing middleware approach was kept instead of restructuring the application.
- Existing business logic, authentication, database configuration, controllers, and services were left unchanged.
- The `/api/test/exception` endpoint is intentionally retained for this hands-on lab and should be removed or disabled before production deployment.
# CardiacPatientMonitoring Day 3

## Overview

This project is the Day 3 implementation of the CardiacPatientMonitoring backend exercise. It focuses on project selection, test setup, and writing unit tests using xUnit.

The goal is to build a realistic healthcare API foundation and verify its business logic through automated tests.

---

## Project Selection

I selected the Healthcare Management API project and named it `CardiacPatientMonitoring` because it matches a realistic backend domain and provides strong practical value.

This project includes features such as:

- patient management
- vital signs tracking
- medication records
- appointments
- authentication

These are common real-world healthcare features and they make the project a suitable capstone candidate.

---

## Scope and Goal

This project is a good Phase 3 backend choice because it already has:

- a clear domain model
- authentication support
- API structure
- realistic healthcare workflows
- testable business logic

The idea is to continue improving it with validation, testing, API documentation, and deployment readiness.

---

## Test Project Setup

An xUnit test project was created named `CardiacPatientMonitoring.Tests`.

The test project references the API project so the tests can directly test the real application code.

```xml
<ItemGroup>
  <ProjectReference Include="..\CardiacPatientMonitoring.Api\CardiacPatientMonitoring.Api.csproj" />
</ItemGroup>
```

This allows us to test real logic from the backend application without duplicating code.

---

## Business Logic Under Test

A simple service was created to calculate a risk score based on patient vital signs.

### RiskScoreCalculator

```csharp
namespace CardiacPatientMonitoring.Api.Services;

public class RiskScoreCalculator
{
    public int CalculateRiskScore(int heartRate, int systolicPressure, int diastolicPressure)
    {
        if (heartRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(heartRate), "Heart rate must be greater than zero.");

        if (systolicPressure <= 0)
            throw new ArgumentOutOfRangeException(nameof(systolicPressure), "Systolic pressure must be greater than zero.");

        if (diastolicPressure <= 0)
            throw new ArgumentOutOfRangeException(nameof(diastolicPressure), "Diastolic pressure must be greater than zero.");

        var score = 0;

        if (heartRate > 100)
            score += 20;

        if (systolicPressure > 140)
            score += 25;

        if (diastolicPressure > 90)
            score += 15;

        return score;
    }
}
```

This logic is simple and representative of a healthcare risk evaluation rule.

---

## Unit Test Examples

### 1. Basic Arrange-Act-Assert test

```csharp
[Fact]
public void CalculateRiskScore_WhenAllVitalsNormal_ReturnsZero()
{
    var calculator = new RiskScoreCalculator();
    var heartRate = 72;
    var systolicPressure = 120;
    var diastolicPressure = 80;

    var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

    Assert.Equal(0, result);
}
```

### 2. Specific rule test

```csharp
[Fact]
public void CalculateRiskScore_WhenHeartRateIsHigh_AddsHeartRatePenalty()
{
    var calculator = new RiskScoreCalculator();
    var heartRate = 110;
    var systolicPressure = 120;
    var diastolicPressure = 80;

    var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

    Assert.Equal(20, result);
}
```

### 3. Multiple input test using [Theory]

```csharp
[Theory]
[InlineData(72, 120, 80, 0)]
[InlineData(110, 120, 80, 20)]
[InlineData(72, 150, 95, 40)]
public void CalculateRiskScore_ForMultipleInputs_ReturnsExpectedScore(int heartRate, int systolicPressure, int diastolicPressure, int expected)
{
    var calculator = new RiskScoreCalculator();

    var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

    Assert.Equal(expected, result);
}
```

---

## Testing Workflow

The testing workflow used in this project is:

1. Arrange the input values.
2. Act by calling the method under test.
3. Assert the expected result.

This is the classic xUnit testing pattern and is the foundation of reliable automated tests.

---

## Verification

The tests were verified with the following command:

```bash
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj --nologo
```

### Result

- total: 17
- passed: 17
- failed: 0
- skipped: 0

This confirms the project is building correctly and the unit tests pass successfully.

---

## Summary

This Day 3 lab focuses on:

- selecting a meaningful project
- creating a test project
- writing clean test cases
- using xUnit and [Fact]/[Theory]
- verifying results through automated execution

The project demonstrates the correct baseline for backend testing and is a strong starting point for the overall healthcare monitoring system.

---

## Files in this folder

- `CardiacPatientMonitoring.Api/`
- `CardiacPatientMonitoring.Tests/`
- `CardiacPatientMonitoring.postman_collection.json`
- `Hands-On Lab.md`
- `README_DAY2.md`
- `README.md`

