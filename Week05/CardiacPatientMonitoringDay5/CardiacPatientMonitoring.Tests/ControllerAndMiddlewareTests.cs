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
    {
        var service = new Mock<IAuthService>();

        service
            .Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
            .ReturnsAsync(
                new AuthResponseDto(
                    "token",
                    DateTime.UtcNow));

        var result = await new AuthController(service.Object)
            .Register(new("a@b.com", "Pass123"));

        Assert.Equal(
            201,
            Assert.IsType<ObjectResult>(result.Result).StatusCode);
    }

    [Fact]
    public async Task Login_InvalidCredentials_PropagatesUnauthorized()
    {
        var service = new Mock<IAuthService>();

        service
            .Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => new AuthController(service.Object)
                .Login(new("a@b.com", "bad")));
    }

    [Fact]
    public async Task Patient_Delete_Returns204()
    {
        var service = new Mock<IPatientService>();

        var result = await new PatientsController(service.Object)
            .Delete(5);

        Assert.IsType<NoContentResult>(result);

        service.Verify(
            x => x.DeleteAsync(5),
            Times.Once);
    }

    [Fact]
    public async Task Patient_GetNotFound_Propagates()
    {
        var service = new Mock<IPatientService>();

        service
            .Setup(x => x.GetAsync(7))
            .ThrowsAsync(
                new NotFoundException("Patient not found."));

        await Assert.ThrowsAsync<NotFoundException>(
            () => new PatientsController(service.Object)
                .Get(7));
    }

    [Fact]
    public async Task Vital_Create_Returns201()
    {
        var service = new Mock<IVitalSignService>();

        service
            .Setup(x => x.CreateAsync(
                1,
                It.IsAny<VitalSignRequestDto>()))
            .ReturnsAsync(
                new VitalSignResponseDto(
                    2,
                    1,
                    70,
                    120,
                    80,
                    null,
                    DateTime.UtcNow));

        var result = await new VitalSignsController(service.Object)
            .Create(1, new());

        Assert.Equal(
            201,
            Assert.IsType<CreatedAtActionResult>(result.Result).StatusCode);
    }

    [Fact]
    public async Task Medication_DeleteNotFound_Propagates()
    {
        var service = new Mock<IMedicationService>();

        service
            .Setup(x => x.DeleteAsync(3))
            .ThrowsAsync(
                new NotFoundException("Medication not found."));

        await Assert.ThrowsAsync<NotFoundException>(
            () => new MedicationsController(service.Object)
                .Delete(3));
    }

    [Fact]
    public async Task Appointment_Create_Returns201()
    {
        var service = new Mock<IAppointmentService>();

        service
            .Setup(x => x.CreateAsync(
                1,
                It.IsAny<AppointmentRequestDto>()))
            .ReturnsAsync(
                new AppointmentResponseDto(
                    2,
                    1,
                    DateTime.UtcNow,
                    "Dr",
                    "Review",
                    "Scheduled"));

        var result = await new AppointmentsController(service.Object)
            .Create(1, new());

        Assert.Equal(
            201,
            Assert.IsType<CreatedAtActionResult>(result.Result).StatusCode);
    }

    [Fact]
    public async Task Middleware_MapsNotFoundTo404()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new NotFoundException("missing"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.InvokeAsync(context);

        Assert.Equal(404, context.Response.StatusCode);
    }

    [Fact]
    public async Task Middleware_MapsUnauthorizedTo401()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new UnauthorizedAccessException("bad"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.InvokeAsync(context);

        Assert.Equal(401, context.Response.StatusCode);
    }

    [Fact]
    public async Task Middleware_MapsUnexpectedExceptionToSafeProblemDetails()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException(
                "database password: secret"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;

        var body = await new StreamReader(
            context.Response.Body)
            .ReadToEndAsync();

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            context.Response.ContentType);

        Assert.Contains(
            "An unexpected error occurred.",
            body);

        Assert.DoesNotContain(
            "database password",
            body);

        Assert.DoesNotContain(
            "secret",
            body);

        Assert.DoesNotContain(
            "InvalidOperationException",
            body);

        Assert.DoesNotContain(
            "StackTrace",
            body);
    }
}