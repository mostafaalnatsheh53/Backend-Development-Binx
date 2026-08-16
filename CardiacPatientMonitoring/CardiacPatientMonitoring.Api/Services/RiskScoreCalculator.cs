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
