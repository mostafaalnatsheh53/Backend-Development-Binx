using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Models;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Tests;

public class VitalSignServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidReading_PersistsAndReturnsVitalSign()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(new Patient { Id = 1, FirstName = "Alex", LastName = "Taylor" });
        await db.SaveChangesAsync();
        var service = new VitalSignService(db);
        var recordedAt = DateTime.UtcNow.AddMinutes(-1);
        var request = new VitalSignRequestDto
        {
            HeartRate = 72,
            SystolicBloodPressure = 120,
            DiastolicBloodPressure = 80,
            TemperatureCelsius = 36.8m,
            RecordedAt = recordedAt
        };

        var response = await service.CreateAsync(1, request);

        Assert.Equal(1, response.PatientId);
        Assert.Equal(72, response.HeartRate);
        Assert.Equal(120, response.SystolicBloodPressure);
        Assert.Equal(80, response.DiastolicBloodPressure);
        Assert.Single(db.VitalSigns);
    }

    [Fact]
    public async Task CreateAsync_WhenDiastolicPressureIsNotLower_ThrowsArgumentException()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(new Patient { Id = 1, FirstName = "Alex", LastName = "Taylor" });
        await db.SaveChangesAsync();
        var service = new VitalSignService(db);
        var request = CreateRequest(systolicPressure: 120, diastolicPressure: 120);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(1, request));

        Assert.Contains("Diastolic blood pressure", exception.Message);
        Assert.Empty(db.VitalSigns);
    }

    [Fact]
    public async Task CreateAsync_WhenRecordedAtIsTooFarInFuture_ThrowsArgumentException()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(new Patient { Id = 1, FirstName = "Alex", LastName = "Taylor" });
        await db.SaveChangesAsync();
        var service = new VitalSignService(db);
        var request = CreateRequest(recordedAt: DateTime.UtcNow.AddMinutes(6));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(1, request));

        Assert.Empty(db.VitalSigns);
    }

    private static VitalSignRequestDto CreateRequest(
        decimal systolicPressure = 120,
        decimal diastolicPressure = 80,
        DateTime? recordedAt = null) => new()
        {
            HeartRate = 72,
            SystolicBloodPressure = systolicPressure,
            DiastolicBloodPressure = diastolicPressure,
            RecordedAt = recordedAt ?? DateTime.UtcNow.AddMinutes(-1)
        };

    private static ApplicationDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
