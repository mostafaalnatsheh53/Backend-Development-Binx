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
    public DbSet<CareCategory> CareCategories => Set<CareCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

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

        b.Entity<CareCategory>()
            .Property(c => c.Name)
            .HasMaxLength(80)
            .IsRequired();

        b.Entity<Product>()
            .Property(p => p.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.Entity<Product>()
            .Property(p => p.StockQuantity)
            .IsRequired();

        b.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Entity<Order>()
            .Property(o => o.Status)
            .HasMaxLength(30)
            .IsRequired();

        b.Entity<Order>()
            .Property(o => o.OrderTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Entity<OrderItem>()
            .Property(i => i.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Patient>()
            .HasMany(p => p.VitalSigns)
            .WithOne(v => v.Patient)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Patient>()
            .HasMany(p => p.Medications)
            .WithOne(m => m.Patient)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Patient>()
            .HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

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

        b.Entity<CareCategory>().HasData(
            new CareCategory { Id = 1, Name = "Low Risk", Description = "Stable patient with no immediate concern." },
            new CareCategory { Id = 2, Name = "Moderate Risk", Description = "Requires monitoring and periodic review." },
            new CareCategory { Id = 3, Name = "High Risk", Description = "Urgent follow-up or escalation is recommended." }
        );

        b.Entity<Product>().HasData(new Product
        {
            Id = 1,
            Name = "Remote ECG Monitor",
            UnitPrice = 125.00m,
            StockQuantity = 5
        });
    }
}