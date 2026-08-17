# CardiacPatientMonitoring Day 2 - Mocking and Dependency Injection

## Overview

This lab focuses on the practical use of dependency injection and Moq in unit testing. The goal is to test controller behavior without connecting to the real database.

The project uses interfaces for business services and injects their concrete implementations through ASP.NET Core. In tests, those implementations are replaced by mocked objects.

---

## 1. Dependency Injection

In ASP.NET Core, services are registered in `Program.cs`:

```csharp
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVitalSignService, VitalSignService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
```

This means the app resolves the real implementation automatically when a controller asks for an interface.

### Why it matters

- We depend on abstractions, not concrete classes.
- The code is easier to test.
- The implementation can be replaced in tests.

---

## 2. Controllers Depend on Interfaces

The controller receives the service through the interface:

```csharp
public class PatientsController(IPatientService s) : ControllerBase
{
    public async Task<IActionResult> Delete(int id)
    {
        await s.DeleteAsync(id);
        return NoContent();
    }
}
```

This is the core idea behind mocking: the controller does not care whether the real service or a fake service is used.

---

## 3. Mocking with Moq

Moq creates a fake implementation of an interface at runtime.

```csharp
var service = new Mock<IPatientService>();
```

Then we configure the mock:

```csharp
service.Setup(s => s.CreateAsync(It.IsAny<PatientRequestDto>()))
    .ReturnsAsync(new PatientResponseDto(4, "Sam", "Lee", new DateOnly(1990, 1, 1), "Female", null));
```

This tells the mock:
- when `CreateAsync` is called,
- return this specific object.

The mock object is passed to the controller using `.Object`:

```csharp
new PatientsController(service.Object)
```

---

## 4. Test: Return a Value

Example from the project:

```csharp
[Fact]
public async Task Create_ReturnsCreatedResponse()
{
    var service = new Mock<IPatientService>();

    service.Setup(s => s.CreateAsync(It.IsAny<PatientRequestDto>()))
        .ReturnsAsync(new PatientResponseDto(4, "Sam", "Lee", new DateOnly(1990, 1, 1), "Female", null));

    var result = await new PatientsController(service.Object)
        .Create(new PatientRequestDto
        {
            FirstName = "Sam",
            LastName = "Lee",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Gender = "Female"
        });

    var created = Assert.IsType<CreatedAtActionResult>(result.Result);
    Assert.Equal(201, created.StatusCode);
}
```

### Purpose

This proves that the controller processes the returned value correctly without using the real database.

---

## 5. Test: Throw an Exception

Example from the project:

```csharp
[Fact]
public async Task Get_WhenServiceCannotFindPatient_ThrowsNotFound()
{
    var service = new Mock<IPatientService>();
    service.Setup(s => s.GetAsync(99))
        .ThrowsAsync(new NotFoundException("Patient not found."));

    await Assert.ThrowsAsync<NotFoundException>(() =>
        new PatientsController(service.Object).Get(99));
}
```

### Purpose

This tests the failure path. The mock deliberately throws an exception, so we can verify that the controller handles failure correctly.

---

## 6. Verify the Interaction

Moq can verify whether a method was called and how many times:

```csharp
s.Verify(x => x.DeleteAsync(5), Times.Once);
```

Example:

```csharp
[Fact]
public async Task Patient_Delete_Returns204()
{
    var s = new Mock<IPatientService>();
    var r = await new PatientsController(s.Object).Delete(5);

    Assert.IsType<NoContentResult>(r);
    s.Verify(x => x.DeleteAsync(5), Times.Once);
}
```

### Purpose

This confirms the correct operation happened exactly once, and it prevents hidden bugs like skipped or duplicated calls.

---

## 7. Important Concept

A mock is useful because it allows us to test logic in isolation.

Without mocking:
- the test would hit a real database
- it would be slower
- it would be less reliable
- it would be harder to reproduce edge cases

With mocking:
- the test is fast
- the test is focused
- we control the exact input and output
- we can simulate both success and failure paths

---

## 8. Project Files Used

- [CardiacPatientMonitoring.Api/Program.cs](CardiacPatientMonitoring.Api/Program.cs)
- [CardiacPatientMonitoring.Api/Controllers/Controllers.cs](CardiacPatientMonitoring.Api/Controllers/Controllers.cs)
- [CardiacPatientMonitoring.Api/Services/Services.cs](CardiacPatientMonitoring.Api/Services/Services.cs)
- [CardiacPatientMonitoring.Tests/PatientsControllerTests.cs](CardiacPatientMonitoring.Tests/PatientsControllerTests.cs)
- [CardiacPatientMonitoring.Tests/ControllerAndMiddlewareTests.cs](CardiacPatientMonitoring.Tests/ControllerAndMiddlewareTests.cs)

---

## 9. Summary

This day introduced the following ideas:

- Interface-based dependency injection
- Mocking dependencies with Moq
- Returning fake data in tests
- Throwing exceptions in tests
- Verifying interaction calls with `.Verify()`

This is a core unit testing pattern used in modern ASP.NET Core applications.
