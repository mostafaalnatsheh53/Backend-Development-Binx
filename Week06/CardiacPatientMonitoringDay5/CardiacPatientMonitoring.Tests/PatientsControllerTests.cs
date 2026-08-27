using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class PatientsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsPagedCatalogResponse()
    {
        var service = new Mock<IPatientService>();
        var expected = new PagedResponseDto<PatientResponseDto>(
            [new PatientResponseDto(1, "Sam", "Lee", new DateOnly(1990, 1, 1), "Female", null)],
            1,
            10,
            1,
            1);
        service.Setup(s => s.GetAllAsync(It.IsAny<PatientCatalogQuery>()))
            .ReturnsAsync(expected);

        var result = await new PatientsController(service.Object).GetAll(
            new PatientCatalogQuery(1, 10, null, "Female", null, "nameAsc"));

        Assert.Same(expected, result);
        service.Verify(s => s.GetAllAsync(It.Is<PatientCatalogQuery>(q =>
            q.Page == 1 && q.PageSize == 10 && q.Gender == "Female" && q.Sort == "nameAsc")), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResponse()
    {
        // Mock the service dependency and return a fake response.
        var service = new Mock<IPatientService>();
        service.Setup(s => s.CreateAsync(It.IsAny<PatientRequestDto>()))
            .ReturnsAsync(new PatientResponseDto(4, "Sam", "Lee", new DateOnly(1990, 1, 1), "Female", null));

        // Act
        var result = await new PatientsController(service.Object)
            .Create(new PatientRequestDto { FirstName = "Sam", LastName = "Lee", DateOfBirth = new DateOnly(1990, 1, 1), Gender = "Female" });

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, created.StatusCode);
    }

    [Fact]
    public async Task Get_WhenServiceCannotFindPatient_ThrowsNotFound()
    {
        // Mock the repository/service behavior for the not-found case.
        var service = new Mock<IPatientService>();
        service.Setup(s => s.GetAsync(99)).ThrowsAsync(new NotFoundException("Patient not found."));

        await Assert.ThrowsAsync<NotFoundException>(() => new PatientsController(service.Object).Get(99));
    }
}
