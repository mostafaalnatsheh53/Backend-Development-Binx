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
        //what is arrange?
        // Arrange is the first step in a unit test where you set up the necessary objects, variables, and state needed for the test. It involves preparing the environment and inputs that will be used in the test. In this case, it includes creating an instance of the RiskScoreCalculator and defining the heart rate, systolic pressure, and diastolic pressure values that will be passed to the method being tested.
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
    [InlineData(110, 150, 95, 60)]
    public void CalculateRiskScore_ForMultipleInputs_ReturnsExpectedScore(int heartRate, int systolicPressure, int diastolicPressure, int expected)
    {
        // Arrange
        var calculator = new RiskScoreCalculator();

        // Act
        var result = calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 120, 80, "heartRate")]
    [InlineData(72, 0, 80, "systolicPressure")]
    [InlineData(72, 120, 0, "diastolicPressure")]
    public void CalculateRiskScore_WhenVitalIsInvalid_ThrowsArgumentOutOfRangeException(
        int heartRate,
        int systolicPressure,
        int diastolicPressure,
        string parameterName)
    {
        var calculator = new RiskScoreCalculator();

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            calculator.CalculateRiskScore(heartRate, systolicPressure, diastolicPressure));

        Assert.Equal(parameterName, exception.ParamName);
    }
}
