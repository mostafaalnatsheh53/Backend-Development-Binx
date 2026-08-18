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

