using Microsoft.AspNetCore.Identity;

namespace CardiacPatientMonitoring.Api.Models;

public class ApplicationUser : IdentityUser
{
}

public class Patient
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();

    public ICollection<Medication> Medications { get; set; } = new List<Medication>();

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

public class VitalSign
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public decimal HeartRate { get; set; }

    public decimal SystolicBloodPressure { get; set; }

    public decimal DiastolicBloodPressure { get; set; }

    public decimal? TemperatureCelsius { get; set; }

    public DateTime RecordedAt { get; set; }

    public Patient Patient { get; set; } = null!;
}

public class Medication
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Dosage { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public Patient Patient { get; set; } = null!;
}

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string ClinicianName { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "Scheduled";

    public Patient Patient { get; set; } = null!;
}