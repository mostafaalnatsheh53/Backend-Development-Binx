using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Models;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Identity;
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
        var service = new AuthService(users.Object, CreateConfiguration().Object);

        var response = await service.LoginAsync(new LoginDto(user.Email, "Pass123"));
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);

            Assert.Equal(
                "user-1",
                token.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(
                user.Email,
                token.Claims.Single(x => x.Type == ClaimTypes.Email).Value);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ThrowsUnauthorizedAccessException()
    {
        var user = new ApplicationUser { Id = "user-1", Email = "doctor@example.com" };
        var users = CreateUserManager();
        users.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        users.Setup(x => x.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);
        var service = new AuthService(users.Object, CreateConfiguration().Object);

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
        var service = new AuthService(users.Object, CreateConfiguration().Object);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.RegisterAsync(new RegisterDto("doctor@example.com", "weak")));

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

    private static Mock<IConfiguration> CreateConfiguration()
    {
        var jwt = new Mock<IConfigurationSection>();
        jwt.Setup(x => x["Issuer"]).Returns("CardiacIssuer");
        jwt.Setup(x => x["Audience"]).Returns("CardiacAudience");
        jwt.Setup(x => x["Key"]).Returns("a-development-key-that-is-long-enough");

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(x => x.GetSection("Jwt")).Returns(jwt.Object);
        return configuration;
    }
}
