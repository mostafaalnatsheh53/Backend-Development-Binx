# Hands-On Lab: Choose Your Project & Write First Unit Tests

## 1. Project Selection

I selected the Healthcare Management API project and named it CardiacPatientMonitoring because it matches my backend interests and the healthcare domain I want to develop further. This project is a strong Phase 3 choice because it includes patient management, vital signs, medications, appointments, and authentication, which are realistic business features for a healthcare system.

## 2. Scope Statement

This project can realistically deliver the professional baseline by Week 9 because it already has the core API structure, authentication, data models, and testing patterns needed for a complete backend solution. By expanding the current foundation with validation, role-based access control, migration documentation, Postman coverage, deployment, and CI/CD, it can become a production-ready capstone. The project is realistic, testable, and scalable, which makes it a suitable choice for the final Phase 3 submission.

## 3. xUnit Test Project Setup

I created the xUnit test project named CardiacPatientMonitoring.Tests and added a ProjectReference to the API project, CardiacPatientMonitoring.Api, so the tests can access the real application code.

### Project reference

```xml
<ItemGroup>
  <ProjectReference Include="..\CardiacPatientMonitoring.Api\CardiacPatientMonitoring.Api.csproj" />
</ItemGroup>
```

The project file is located here:
- CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj

## 4. Three [Fact] Tests using Arrange-Act-Assert

I created a simple pure service in the API project:
- CardiacPatientMonitoring.Api/Services/RiskScoreCalculator.cs

### Service code

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

### Test file

```csharp
using CardiacPatientMonitoring.Api.Services;

namespace CardiacPatientMonitoring.Tests;

public class RiskScoreCalculatorTests
{
    [Fact]
    public void CalculateRiskScore_WhenAllVitalsNormal_ReturnsZero()
    {
        // Arrange
        var calculator = new RiskScoreCalculator();
        var heartRate = 72;
        var systolicPressure = 120;
        var diastolicPressure = 80;

        // Act
        var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateRiskScore_WhenHeartRateIsHigh_AddsHeartRatePenalty()
    {
        // Arrange
        var calculator = new RiskScoreCalculator();
        var heartRate = 110;
        var systolicPressure = 120;
        var diastolicPressure = 80;

        // Act
        var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

        // Assert
        Assert.Equal(20, result);
    }

    [Fact]
    public void CalculateRiskScore_WhenBloodPressureIsHigh_AddsPressurePenalty()
    {
        // Arrange
        var calculator = new RiskScoreCalculator();
        var heartRate = 72;
        var systolicPressure = 150;
        var diastolicPressure = 95;

        // Act
        var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

        // Assert
        Assert.Equal(40, result);
    }
}
```

## 5. One [Theory] Test with Multiple Input Cases

```csharp
[Theory]
[InlineData(72, 120, 80, 0)]
[InlineData(110, 120, 80, 20)]
[InlineData(72, 150, 95, 40)]
public void CalculateRiskScore_ForMultipleInputs_ReturnsExpectedScore(int heartRate, int systolicPressure, int diastolicPressure, int expected)
{
    // Arrange
    var calculator = new RiskScoreCalculator();

    // Act
    var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

    // Assert
    Assert.Equal(expected, result);
}
```

## Verification

I verified the solution by running:

```bash
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj --nologo
```

The result was successful:
- Total tests: 17
- Passed: 17
- Failed: 0
- Skipped: 0

## Summary

This lab demonstrates the proper workflow for a Phase 3 capstone project: selecting a suitable project, defining a realistic scope, creating an xUnit test project, writing clean Arrange-Act-Assert tests, and validating everything through automated execution.
