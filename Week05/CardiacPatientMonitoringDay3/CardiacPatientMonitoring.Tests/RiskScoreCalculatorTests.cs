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
}
