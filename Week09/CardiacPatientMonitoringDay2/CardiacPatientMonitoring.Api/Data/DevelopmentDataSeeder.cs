using CardiacPatientMonitoring.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public static class DevelopmentDataSeeder
{
    private const int TargetPatientCount = 50;
    private const string PatientUserIdPrefix = "d0000000-0000-0000-0000-0000000000";

    public static async Task SeedAsync(ApplicationDbContext db)
    {
        var patientCount = await db.Patients.CountAsync();
        if (patientCount >= TargetPatientCount)
            return;

        for (var patientNumber = 2; patientNumber <= TargetPatientCount; patientNumber++)
        {
            var userId = $"{PatientUserIdPrefix}{patientNumber:0000}";
            if (await db.Patients.AnyAsync(p => p.UserId == userId))
                continue;

            if (!await db.Users.AnyAsync(u => u.Id == userId))
            {
                db.Users.Add(new ApplicationUser
                {
                    Id = userId,
                    UserName = $"patient{patientNumber}@example.invalid",
                    NormalizedUserName = $"PATIENT{patientNumber}@EXAMPLE.INVALID",
                    Email = $"patient{patientNumber}@example.invalid",
                    NormalizedEmail = $"PATIENT{patientNumber}@EXAMPLE.INVALID",
                    EmailConfirmed = true,
                    SecurityStamp = $"seed-security-{patientNumber:0000}",
                    ConcurrencyStamp = $"seed-concurrency-{patientNumber:0000}"
                });
            }

            db.Patients.Add(new Patient
            {
                FirstName = GetFirstName(patientNumber),
                LastName = GetLastName(patientNumber),
                DateOfBirth = new DateOnly(1970 + patientNumber % 30, (patientNumber % 12) + 1, (patientNumber % 27) + 1),
                Gender = patientNumber % 2 == 0 ? "Female" : "Male",
                PhoneNumber = $"555-{patientNumber:0000}",
                UserId = userId,
                VitalSigns =
                {
                    new VitalSign
                    {
                        HeartRate = 65 + patientNumber % 20,
                        SystolicBloodPressure = 110 + patientNumber % 20,
                        DiastolicBloodPressure = 70 + patientNumber % 10,
                        TemperatureCelsius = 36.5m + (patientNumber % 5) * 0.1m,
                        RecordedAt = DateTime.UtcNow.AddDays(-patientNumber)
                    }
                },
                Medications =
                {
                    new Medication
                    {
                        Name = patientNumber % 2 == 0 ? "Lisinopril" : "Atorvastatin",
                        Dosage = patientNumber % 2 == 0 ? "10 mg" : "20 mg",
                        Frequency = "Once daily",
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30))
                    }
                },
                Appointments =
                {
                    new Appointment
                    {
                        ScheduledAt = DateTime.UtcNow.AddDays(patientNumber),
                        ClinicianName = patientNumber % 2 == 0 ? "Dr. Morgan" : "Dr. Patel",
                        Reason = "Cardiac follow-up",
                        Status = patientNumber % 3 == 0 ? "Completed" : "Scheduled"
                    }
                }
            });
        }

        await db.SaveChangesAsync();
    }

    private static string GetFirstName(int patientNumber) =>
        new[] { "Jordan", "Taylor", "Morgan", "Casey", "Riley" }[patientNumber % 5];

    private static string GetLastName(int patientNumber) =>
        new[] { "Bennett", "Rivera", "Chen", "Patel", "Brooks" }[patientNumber % 5];
}