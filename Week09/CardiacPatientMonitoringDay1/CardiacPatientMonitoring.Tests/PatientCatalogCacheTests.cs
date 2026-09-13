using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Models;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CardiacPatientMonitoring.Tests;

public class PatientCatalogCacheTests
{
    [Fact]
    public async Task GetAllAsync_UsesCachedCatalogUntilInvalidated()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(CreatePatient(1, "Alex"));
        await db.SaveChangesAsync();
        var cache = CreateCache();
        var service = CreateService(db, cache);

        var firstResult = await service.GetAllAsync(null);
        db.Patients.Add(CreatePatient(2, "Jamie"));
        await db.SaveChangesAsync();

        var cachedResult = await service.GetAllAsync(null);

        Assert.Single(firstResult);
        Assert.Single(cachedResult);
        Assert.Equal("Alex", cachedResult.Single().FirstName);
    }

    [Fact]
    public async Task UpdateAsync_InvalidatesCatalogCache()
    {
        await using var db = CreateDatabase();
        db.Patients.Add(CreatePatient(1, "Alex"));
        await db.SaveChangesAsync();
        var cache = CreateCache();
        var service = CreateService(db, cache);

        await service.GetAllAsync(null);
        await service.UpdateAsync(1, new PatientRequestDto
        {
            FirstName = "Updated",
            LastName = "Taylor",
            DateOfBirth = new DateOnly(1982, 5, 14),
            Gender = "Other"
        });

        var freshResult = await service.GetAllAsync(null);

        Assert.Equal("Updated", freshResult.Single().FirstName);
    }

    private static PatientService CreateService(
        ApplicationDbContext db,
        IDistributedCache cache) =>
        new(db, cache, new ConfigurationBuilder().Build());

    private static IDistributedCache CreateCache() =>
        new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));

    private static Patient CreatePatient(int id, string firstName) => new()
    {
        Id = id,
        FirstName = firstName,
        LastName = "Taylor",
        DateOfBirth = new DateOnly(1982, 5, 14),
        Gender = "Other"
    };

    private static ApplicationDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}