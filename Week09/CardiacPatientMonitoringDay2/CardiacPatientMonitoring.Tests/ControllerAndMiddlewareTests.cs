using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Middleware;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class ControllerAndMiddlewareTests
{
    [Fact]
    public async Task Register_Returns201AndToken()
    { var s = new Mock<IAuthService>(); s.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>())).ReturnsAsync(new AuthResponseDto("token", DateTime.UtcNow)); var r = await new AuthController(s.Object).Register(new("a@b.com", "Pass123")); Assert.Equal(201, Assert.IsType<ObjectResult>(r.Result).StatusCode); }
    [Fact]
    public async Task Login_InvalidCredentials_PropagatesUnauthorized()
    { var s = new Mock<IAuthService>(); s.Setup(x => x.LoginAsync(It.IsAny<LoginDto>())).ThrowsAsync(new UnauthorizedAccessException()); await Assert.ThrowsAsync<UnauthorizedAccessException>(() => new AuthController(s.Object).Login(new("a@b.com", "bad"))); }
    [Fact]
    public async Task Patient_Delete_Returns204()
    { var s = new Mock<IPatientService>(); var r = await new PatientsController(s.Object).Delete(5); Assert.IsType<NoContentResult>(r); s.Verify(x => x.DeleteAsync(5), Times.Once); }
    [Fact]
    public async Task Patient_GetNotFound_Propagates()
    { var s = new Mock<IPatientService>(); s.Setup(x => x.GetAsync(7)).ThrowsAsync(new NotFoundException("Patient not found.")); await Assert.ThrowsAsync<NotFoundException>(() => new PatientsController(s.Object).Get(7)); }
    [Fact]
    public async Task Vital_Create_Returns201()
    { var s = new Mock<IVitalSignService>(); s.Setup(x => x.CreateAsync(1, It.IsAny<VitalSignRequestDto>())).ReturnsAsync(new VitalSignResponseDto(2, 1, 70, 120, 80, null, DateTime.UtcNow)); var r = await new VitalSignsController(s.Object).Create(1, new()); Assert.Equal(201, Assert.IsType<CreatedAtActionResult>(r.Result).StatusCode); }
    [Fact]
    public async Task Medication_DeleteNotFound_Propagates()
    { var s = new Mock<IMedicationService>(); s.Setup(x => x.DeleteAsync(3)).ThrowsAsync(new NotFoundException("Medication not found.")); await Assert.ThrowsAsync<NotFoundException>(() => new MedicationsController(s.Object).Delete(3)); }
    [Fact]
    public async Task Appointment_Create_Returns201()
    { var s = new Mock<IAppointmentService>(); s.Setup(x => x.CreateAsync(1, It.IsAny<AppointmentRequestDto>())).ReturnsAsync(new AppointmentResponseDto(2, 1, DateTime.UtcNow, "Dr", "Review", "Scheduled")); var r = await new AppointmentsController(s.Object).Create(1, new()); Assert.Equal(201, Assert.IsType<CreatedAtActionResult>(r.Result).StatusCode); }
    [Fact]
    public async Task Middleware_MapsNotFoundTo404()
    { var m = new ExceptionHandlingMiddleware(_ => throw new NotFoundException("missing"), NullLogger<ExceptionHandlingMiddleware>.Instance); var c = new DefaultHttpContext(); c.Response.Body = new MemoryStream(); await m.InvokeAsync(c); Assert.Equal(404, c.Response.StatusCode); }
    [Fact]
    public async Task Middleware_MapsUnauthorizedTo401()
    { var m = new ExceptionHandlingMiddleware(_ => throw new UnauthorizedAccessException("bad"), NullLogger<ExceptionHandlingMiddleware>.Instance); var c = new DefaultHttpContext(); c.Response.Body = new MemoryStream(); await m.InvokeAsync(c); Assert.Equal(401, c.Response.StatusCode); }

    [Fact]
    public async Task Middleware_MapsUnexpectedExceptionToSafeProblemDetails()
    {
        var m = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("database password: secret"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var c = new DefaultHttpContext();
        c.Response.Body = new MemoryStream();

        await m.InvokeAsync(c);

        c.Response.Body.Position = 0;
        var body = await new StreamReader(c.Response.Body).ReadToEndAsync();

        Assert.Equal(500, c.Response.StatusCode);
        Assert.Equal("application/problem+json", c.Response.ContentType);
        Assert.Contains("An unexpected error occurred.", body);
        Assert.DoesNotContain("database password", body);
        Assert.DoesNotContain("secret", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        Assert.DoesNotContain("StackTrace", body);
    }
}
