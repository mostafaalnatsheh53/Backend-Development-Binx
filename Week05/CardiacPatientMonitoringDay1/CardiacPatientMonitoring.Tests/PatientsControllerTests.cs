using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class PatientsControllerTests
{
    [Fact]
    public async Task Create_ReturnsCreatedResponse()
    {
        // Arrange

        var service = new Mock<IPatientService>();

        service.Setup(s => s.CreateAsync(It.IsAny<PatientRequestDto>())).ReturnsAsync(new PatientResponseDto(4, "Sam", "Lee", new DateOnly(1990, 1, 1), "Female", null));
        // Act

        var result = await new PatientsController(service.Object).Create(new PatientRequestDto { FirstName = "Sam", LastName = "Lee", DateOfBirth = new DateOnly(1990, 1, 1), Gender = "Female" });
        // Assert

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
    }
    [Fact]
    public async Task Get_WhenServiceCannotFindPatient_ThrowsNotFound()
    {
        var service = new Mock<IPatientService>();
        service.Setup(s => s.GetAsync(99)).ThrowsAsync(new NotFoundException("Patient not found."));
        await Assert.ThrowsAsync<NotFoundException>(() => new PatientsController(service.Object).Get(99));
    }
}
