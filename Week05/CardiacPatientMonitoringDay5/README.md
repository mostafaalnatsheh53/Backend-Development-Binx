# Week 5 - Cardiac Patient Monitoring

## Day Overview

This project is the Week 5 starting point for the Phase 3 capstone. The selected project is **Cardiac Patient Monitoring**, a healthcare management backend built with ASP.NET Core Web API.

Week 5 focused on establishing a professional project baseline:

- Reusing the existing Week 1-4 healthcare API.
- Identifying and testing the highest-risk business logic.
- Adding integration coverage for an important protected endpoint.
- Centralizing API error handling.
- Running the complete test suite before the mentor check-in.

## Chosen Project and Initial Scope

The API manages:

- User registration and login.
- JWT authentication.
- Patient records.
- Patient vital signs.
- Medications.
- Appointments.
- SQL Server persistence through Entity Framework Core.
- Swagger/OpenAPI documentation.

The Week 1-4 project was reused instead of creating a duplicate application. It already provided the domain models, controllers, services, database context, migration, authentication configuration, and test project required for the Phase 3 starting skeleton.

## Project Structure

```text
CardiacPatientMonitoringDay5/
|-- CardiacPatientMonitoring.Api/
|   |-- Controllers/
|   |-- Data/
|   |-- DTOs/
|   |-- Middleware/
|   |-- Migrations/
|   |-- Models/
|   |-- Services/
|   |-- Program.cs
|   `-- CardiacPatientMonitoring.Api.csproj
|-- CardiacPatientMonitoring.Tests/
|   |-- AuthServiceTests.cs
|   |-- ControllerAndMiddlewareTests.cs
|   |-- CustomWebApplicationFactory.cs
|   |-- PatientEndpointIntegrationTests.cs
|   |-- PatientsControllerTests.cs
|   |-- RiskScoreCalculatorTests.cs
|   |-- VitalSignServiceTests.cs
|   `-- CardiacPatientMonitoring.Tests.csproj
|-- CardiacPatientMonitoring.slnx
|-- CardiacPatientMonitoring.postman_collection.json
`-- README.md
```

## Testing Strategy

Testing priority followed risk and complexity rather than ease of testing. The three highest-risk pieces of logic in the initial scope were selected for focused unit tests.

### 1. RiskScoreCalculator

This business rule validates vital-sign inputs and adds penalties for:

- Heart rate above 100.
- Systolic pressure above 140.
- Diastolic pressure above 90.

The tests cover normal readings, each penalty, combined penalties, and invalid values.

### 2. AuthService

Authentication is high risk because it controls access to the application and creates security tokens. The tests cover:

- Successful login.
- JWT creation with user ID and email claims.
- Invalid password rejection.
- Registration failure returned by ASP.NET Identity.

### 3. VitalSignService

Vital-sign validation protects the quality of healthcare data. The tests cover:

- Persisting a valid reading.
- Rejecting a diastolic pressure that is not lower than systolic pressure.
- Rejecting a reading recorded too far in the future.

`Microsoft.EntityFrameworkCore.InMemory` is used by unit tests so the service can be tested without requiring a local SQL Server instance.

## Integration Testing

Integration tests use `WebApplicationFactory<Program>` and run the application through HTTP. The Testing environment uses an EF Core InMemory database with the model seed data.

The important protected endpoint is:

```text
GET /api/patients
```

The integration tests verify:

1. A request with a valid JWT returns `200 OK` and the seeded patient data.
2. A request without a JWT returns `401 Unauthorized`.

These tests exercise the real authentication middleware, routing, controller, service, database, and HTTP response pipeline together.

## Centralized Error Handling

`ExceptionHandlingMiddleware` is registered before authentication, authorization, and controller execution. Controllers and services can throw application exceptions without duplicating response logic.

| Exception | HTTP status | API response |
| --- | ---: | --- |
| `NotFoundException` | 404 | Resource not found |
| `ArgumentException` | 400 | Invalid request |
| `UnauthorizedAccessException` | 401 | Authentication required |
| Other exceptions | 500 | Unexpected error |

Responses use the standard `ProblemDetails` format and content type `application/problem+json`.

Unexpected exceptions are logged with the HTTP method and request path. Internal messages, stack traces, database details, and connection strings are not exposed to clients.

## Running the Project

From this directory, restore dependencies and run the API:

```powershell
dotnet restore
dotnet run --project .\CardiacPatientMonitoring.Api\CardiacPatientMonitoring.Api.csproj
```

In Development mode, Swagger is available at:

```text
https://localhost:<port>/swagger
```

The API uses the configured SQL Server connection string in `CardiacPatientMonitoring.Api/appsettings.json` for normal execution.

## Running the Tests

Run the full test suite from the Day5 directory:

```powershell
dotnet test
```

Build the complete solution:

```powershell
dotnet build .\CardiacPatientMonitoring.slnx
```

Run only the integration tests:

```powershell
dotnet test --filter "FullyQualifiedName~PatientEndpointIntegrationTests"
```

## Week 5 Verification

The final full-suite command was:

```powershell
dotnet test --nologo
```

Result:

```text
30 tests passed
0 failed
0 skipped
Build succeeded
```

## Sprint 1 Handoff

The project is ready for Phase 3 Sprint 1. The next sprint should extend the core routes and schema while keeping the Week 5 standards:

- Add focused tests with every new business rule or endpoint.
- Keep centralized error handling for application exceptions.
- Protect endpoints with the appropriate authentication requirements.
- Run `dotnet test` before review and before merging changes.

## Mentor Check-in Summary

The capstone has a working baseline, automated unit and integration coverage, JWT authentication, database persistence, centralized safe error responses, and a repeatable full-suite verification command. The current test suite is green and the project can move into Sprint 1 development.
