using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public record RegisterDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt);

public class PatientRequestDto
{
    [Required, StringLength(80)]
    public string FirstName { get; set; } = "";

    [Required, StringLength(80)]
    public string LastName { get; set; } = "";

    public DateOnly DateOfBirth { get; set; }

    [Required, StringLength(20)]
    public string Gender { get; set; } = "";

    [Phone]
    public string? PhoneNumber { get; set; }
}

public record PatientResponseDto(
    int Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Gender,
    string? PhoneNumber);

public class VitalSignRequestDto
{
    [Range(1, 300)]
    public decimal HeartRate { get; set; }

    [Range(50, 300)]
    public decimal SystolicBloodPressure { get; set; }

    [Range(30, 200)]
    public decimal DiastolicBloodPressure { get; set; }

    [Range(30, 45)]
    public decimal? TemperatureCelsius { get; set; }

    public DateTime RecordedAt { get; set; }
}

public record VitalSignResponseDto(
    int Id,
    int PatientId,
    decimal HeartRate,
    decimal SystolicBloodPressure,
    decimal DiastolicBloodPressure,
    decimal? TemperatureCelsius,
    DateTime RecordedAt);

public class MedicationRequestDto
{
    [Required, StringLength(120)]
    public string Name { get; set; } = "";

    [Required, StringLength(80)]
    public string Dosage { get; set; } = "";

    [Required, StringLength(80)]
    public string Frequency { get; set; } = "";

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}

public record MedicationResponseDto(
    int Id,
    int PatientId,
    string Name,
    string Dosage,
    string Frequency,
    DateOnly StartDate,
    DateOnly? EndDate);

public class AppointmentRequestDto
{
    public DateTime ScheduledAt { get; set; }

    [Required, StringLength(100)]
    public string ClinicianName { get; set; } = "";

    [Required, StringLength(300)]
    public string Reason { get; set; } = "";

    [Required, RegularExpression("^(Scheduled|Completed|Cancelled)$")]
    public string Status { get; set; } = "Scheduled";
}

public record AppointmentResponseDto(
    int Id,
    int PatientId,
    DateTime ScheduledAt,
    string ClinicianName,
    string Reason,
    string Status);