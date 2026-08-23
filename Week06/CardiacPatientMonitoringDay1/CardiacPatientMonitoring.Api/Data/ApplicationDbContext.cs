using CardiacPatientMonitoring.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<VitalSign> VitalSigns => Set<VitalSign>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Patient>()
            .Property(p => p.FirstName)
            .HasMaxLength(80)
            .IsRequired();

        b.Entity<Patient>()
            .Property(p => p.LastName)
            .HasMaxLength(80)
            .IsRequired();

        b.Entity<VitalSign>()
            .Property(v => v.HeartRate)
            .HasPrecision(5, 2);

        b.Entity<VitalSign>()
            .Property(v => v.SystolicBloodPressure)
            .HasPrecision(5, 2);

        b.Entity<VitalSign>()
            .Property(v => v.DiastolicBloodPressure)
            .HasPrecision(5, 2);

        b.Entity<VitalSign>()
            .Property(v => v.TemperatureCelsius)
            .HasPrecision(4, 1);

        b.Entity<Medication>()
            .Property(m => m.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.Entity<Appointment>()
            .Property(a => a.Status)
            .HasMaxLength(30)
            .IsRequired();

        b.Entity<Patient>().HasData(new Patient
        {
            Id = 1,
            FirstName = "Alex",
            LastName = "Taylor",
            DateOfBirth = new DateOnly(1982, 5, 14),
            Gender = "Other",
            PhoneNumber = "555-0101"
        });

        b.Entity<VitalSign>().HasData(new VitalSign
        {
            Id = 1,
            PatientId = 1,
            HeartRate = 72,
            SystolicBloodPressure = 120,
            DiastolicBloodPressure = 80,
            TemperatureCelsius = 36.8m,
            RecordedAt = new DateTime(
                2026, 1, 15, 9, 0, 0, DateTimeKind.Utc)
        });

        b.Entity<Medication>().HasData(new Medication
        {
            Id = 1,
            PatientId = 1,
            Name = "Sample Medication",
            Dosage = "10 mg",
            Frequency = "Once daily",
            StartDate = new DateOnly(2026, 1, 1)
        });

        b.Entity<Appointment>().HasData(new Appointment
        {
            Id = 1,
            PatientId = 1,
            ScheduledAt = new DateTime(
                2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
            ClinicianName = "Dr. Morgan",
            Reason = "Routine review",
            Status = "Scheduled"
        });
    }
}