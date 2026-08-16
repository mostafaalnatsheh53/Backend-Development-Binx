# Cardiac Patient Monitoring System API — Phase 3 Day 1 Lab

## Overview

This is a complete ASP.NET Core REST API for cardiac patient monitoring, developed as the Phase 3 capstone project. The project was built following the Day 1 curriculum: project selection, scope definition, xUnit test setup, and writing unit tests using Arrange-Act-Assert pattern.

All included records are synthetic and must not be treated as clinical data.

## Lab Completion Status

✅ **All 5 Lab Requirements Completed**

1. ✅ Project Selection: Healthcare Management API (CardiacPatientMonitoring)
2. ✅ Scope Statement: 3-sentence summary confirming Week 9 deliverables
3. ✅ xUnit Test Project: CardiacPatientMonitoring.Tests with ProjectReference to API
4. ✅ Three [Fact] Tests: RiskScoreCalculator pure service testing
5. ✅ One [Theory] Test: Multiple input cases validation

**Test Results:** 17/17 tests passing ✓

## Stack and Structure

C# / ASP.NET Core 10, EF Core with SQL Server, ASP.NET Core Identity + JWT, OpenAPI, xUnit, and Moq. The API project uses `Controllers`, `Models`, `DTOs`, `Data`, `Services`, and `Middleware`; tests are in `CardiacPatientMonitoring.Tests`.

## Configure and run

1. The default connection targets a local SQL Server Express instance (`.\\SQLEXPRESS`). Update `ConnectionStrings:DefaultConnection` in `CardiacPatientMonitoring.Api/appsettings.json` if yours uses another instance.
2. Replace the development `Jwt:Key` with a secret via user secrets before non-local use.
3. Run `dotnet ef database update --project CardiacPatientMonitoring.Api`.
4. Run `dotnet run --project CardiacPatientMonitoring.Api` and open the development OpenAPI endpoint shown by the app.

The migration seeds one synthetic patient with a vital sign, medication, and appointment. Use `dotnet test` to run tests.

---

## Lab Implementation Details

### 1. Project Selection: Healthcare Management API

**Project Name:** CardiacPatientMonitoring

**Rationale:** Selected because it matches backend interests and the healthcare domain. The project includes patient management, vital signs, medications, appointments, and authentication — realistic business features for a healthcare system.

**Phase 3 Suitability:** This project can realistically deliver the professional baseline by Week 9 because it already has the core API structure, authentication, data models, and testing patterns needed for a complete backend solution. By expanding with validation, role-based access control, migration documentation, Postman coverage, deployment, and CI/CD, it becomes a production-ready capstone.

### 2. Scope Statement

This project can realistically deliver the professional baseline by Week 9 because it already has the core API structure, authentication, data models, and testing patterns needed for a complete backend solution. By expanding the current foundation with validation, role-based access control, migration documentation, Postman coverage, deployment, and CI/CD, it can become a production-ready capstone. The project is realistic, testable, and scalable, which makes it a suitable choice for the final Phase 3 submission.

### 3. xUnit Test Project Setup

**Test Project:** `CardiacPatientMonitoring.Tests`

**Project Reference:** The test project includes a ProjectReference to the API project, allowing tests to access real application code.

```xml
<ItemGroup>
  <ProjectReference Include="..\CardiacPatientMonitoring.Api\CardiacPatientMonitoring.Api.csproj" />
</ItemGroup>
```

**Dependencies:**
- xUnit 2.9.3
- Moq 4.20.72
- Microsoft.NET.Test.Sdk 17.14.1
- xunit.runner.visualstudio 3.1.4

### 4. Pure Service: RiskScoreCalculator

**Location:** `CardiacPatientMonitoring.Api/Services/RiskScoreCalculator.cs`

This is a simple, pure service with no external dependencies. It calculates a cardiac risk score based on vital signs.

```csharp
public class RiskScoreCalculator
{
    public int CalculateRiskScore(int heartRate, int systolicPressure, int diastolicPressure)
    {
        // Validates inputs
        // Calculates risk score based on thresholds
        // Returns integer score (0-60)
    }
}
```

### 5. Three [Fact] Tests

**File:** `CardiacPatientMonitoring.Tests/RiskScoreCalculatorTests.cs`

Each test follows Arrange-Act-Assert pattern:

#### Test 1: Normal Vitals
- **Arrange:** Create calculator, set normal vital values (72 bpm, 120/80 mmHg)
- **Act:** Call CalculateRiskScore
- **Assert:** Expect score = 0

#### Test 2: High Heart Rate
- **Arrange:** Create calculator, set high heart rate (110 bpm)
- **Act:** Call CalculateRiskScore
- **Assert:** Expect score = 20 (heart rate penalty)

#### Test 3: High Blood Pressure
- **Arrange:** Create calculator, set high blood pressure (150/95 mmHg)
- **Act:** Call CalculateRiskScore
- **Assert:** Expect score = 40 (systolic + diastolic penalties)

### 6. One [Theory] Test with 3 Input Cases

**File:** `CardiacPatientMonitoring.Tests/RiskScoreCalculatorTests.cs`

Single test method runs 3 times with different inputs via `[InlineData]`:

| Heart Rate | Systolic | Diastolic | Expected Score |
|-----------|----------|-----------|-----------------|
| 72        | 120      | 80        | 0               |
| 110       | 120      | 80        | 20              |
| 72        | 150      | 95        | 40              |

**Benefit:** Reduces code duplication and validates the same logic across multiple scenarios in one method.


## Testing and Verification

### Running All Tests

```bash
cd CardiacPatientMonitoringDay1
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj --nologo
```

### Test Results

```
Test summary: total: 17; failed: 0; succeeded: 17; skipped: 0
Build succeeded ✓
```

### Test Coverage

- **11 existing tests** from the initial CardiacPatientMonitoring project
  - PatientsController tests
  - ControllerAndMiddleware tests
  
- **3 new [Fact] tests** for RiskScoreCalculator pure service
  - Normal vitals scenario
  - High heart rate scenario
  - High blood pressure scenario

- **3 new [Theory] test iterations** covering multiple input cases
  - Same test runs 3 times with different inputs
  - Validates consistent behavior across scenarios

---

## Authentication and endpoints

Register at `POST /api/auth/register`, then log in at `POST /api/auth/login`. Send the returned JWT as `Authorization: Bearer <token>` for protected routes.

- `GET/POST /api/patients`, `GET/PUT/DELETE /api/patients/{id}` (optional `search`)
- `GET/POST /api/patients/{patientId}/vital-signs`; individual vital signs use `/api/vital-signs/{id}`
- `GET/POST /api/patients/{patientId}/medications` (optional `search`); individual medication routes use `/api/medications/{id}`
- `GET/POST /api/patients/{patientId}/appointments` (optional `status`); individual appointment routes use `/api/appointments/{id}`

DTO data annotations return validation 400 responses. A centralized middleware returns safe JSON errors for missing resources and unexpected failures. Postman can use the same routes and bearer token flow.

---

## Local secrets and Swagger

On a new machine set the JWT key with `dotnet user-secrets set "Jwt:Key" "<long-random-secret>" --project CardiacPatientMonitoring.Api`. Do not commit it. Start the API, open `/swagger`, click **Authorize**, and enter `Bearer <token>` returned by login. Import the included Postman collection and set its base URL to the launch profile URL.

---

## Learning Outcomes

By completing this lab, the following concepts were applied:

### xUnit Fundamentals
- **[Fact]:** Tests a single, specific scenario without parametrization
- **[Theory]:** Tests the same logic multiple times with different inputs via `[InlineData]`

### Arrange-Act-Assert Pattern
- **Arrange:** Set up test preconditions and dependencies
- **Act:** Perform the single action being tested
- **Assert:** Verify outcome matches expectations
- Clean separation improves readability and diagnostic clarity

### Pure Service Testing
- Tested a service with no external dependencies
- Fast, isolated unit tests that focus on business logic
- Easy to understand and maintain

### Project Scoping
- Selected a realistic Phase 3 project
- Defined a 3-sentence scope confirming Week 9 deliverability
- Planned for professional baseline: API, database, auth, tests, deployment, CI/CD

---

## Next Steps (Beyond Day 1)

This Day 1 foundation will be expanded in subsequent weeks to deliver the full professional baseline by Week 9:

- **Week 2-3:** Expand test coverage to Controllers and Middleware
- **Week 4-5:** Add integration tests and validation testing
- **Week 6-7:** Implement role-based access control (RBAC)
- **Week 8:** Prepare deployment pipeline (Azure/Railway + CI/CD)
- **Week 9:** Final documentation and capstone submission

---

## Project Files

```
CardiacPatientMonitoringDay1/
├── CardiacPatientMonitoring.Api/          # Main API project
│   ├── Controllers/                       # API endpoints
│   ├── Models/                            # Domain models
│   ├── DTOs/                              # Data transfer objects
│   ├── Services/                          # Business logic (including RiskScoreCalculator)
│   ├── Data/                              # EF Core DbContext and migrations
│   ├── Middleware/                        # Exception handling middleware
│   └── Program.cs                         # Startup configuration
│
├── CardiacPatientMonitoring.Tests/        # xUnit test project
│   ├── RiskScoreCalculatorTests.cs        # Pure service tests (3 [Fact] + 1 [Theory])
│   ├── PatientsControllerTests.cs         # Controller tests
│   ├── ControllerAndMiddlewareTests.cs    # Integration tests
│   └── CardiacPatientMonitoring.Tests.csproj
│
├── CardiacPatientMonitoring.slnx          # Solution file
├── CardiacPatientMonitoring.postman_collection.json  # Postman API collection
├── Hands-On Lab.md                        # Detailed lab documentation
└── README.md                              # This file
```

---

## Summary

✅ Day 1 Hands-On Lab is complete. All 5 requirements have been implemented and verified:

1. Project selected: Healthcare Management API (CardiacPatientMonitoring)
2. Scope statement written: 3 sentences confirming Week 9 deliverability
3. xUnit test project created with API reference
4. 3 [Fact] tests written using Arrange-Act-Assert
5. 1 [Theory] test written with 3 input cases

All 17 tests pass. The project is ready for Week 2 expansion.
