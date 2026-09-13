using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.Models;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsJwtWithUserClaims()
    {
        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "doctor@example.com",
            UserName = "doctor@example.com"
        };
        var users = CreateUserManager();
        users.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        users.Setup(x => x.CheckPasswordAsync(user, "Pass123")).ReturnsAsync(true);
        await using var db = CreateDb();
        db.Patients.Add(new Patient
        {
            Id = 42,
            UserId = user.Id,
            FirstName = "Test",
            LastName = "Patient",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other"
        });
        await db.SaveChangesAsync();
        var service = new AuthService(users.Object, db, CreateConfiguration());

        var response = await service.LoginAsync(new LoginDto(user.Email, "Pass123"));
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);

            Assert.Equal(
                "user-1",
                token.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(
                user.Email,
                token.Claims.Single(x => x.Type == ClaimTypes.Email).Value);
            Assert.Equal(
                "42",
                token.Claims.Single(x => x.Type == "patient_id").Value);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ThrowsUnauthorizedAccessException()
    {
        var user = new ApplicationUser { Id = "user-1", Email = "doctor@example.com" };
        var users = CreateUserManager();
        users.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        users.Setup(x => x.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);
        await using var db = CreateDb();
        var service = new AuthService(users.Object, db, CreateConfiguration());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginDto(user.Email, "wrong")));
    }

    [Fact]
    public async Task RegisterAsync_WhenIdentityRejectsUser_ThrowsArgumentException()
    {
        var users = CreateUserManager();
        users.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "weak"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Description = "Password is too weak."
            }));
        await using var db = CreateDb();
        var service = new AuthService(users.Object, db, CreateConfiguration());

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.RegisterAsync(Registration("doctor@example.com", "weak")));

        Assert.Contains("Password is too weak.", exception.Message);
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        return new Mock<UserManager<ApplicationUser>>(
              store.Object,
              null!,
              null!,
              null!,
              null!,
              null!,
              null!,
              null!,
              null!);
    }

    private static ApplicationDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "CardiacIssuer",
                ["Jwt:Audience"] = "CardiacAudience",
                ["Jwt:Key"] = "a-development-key-that-is-long-enough",
                ["Jwt:ExpirationMinutes"] = "120"
            })
            .Build();

    private static RegisterDto Registration(string email, string password) =>
        new(email, password)
        {
            FirstName = "Test",
            LastName = "Patient",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Other"
        };
}
