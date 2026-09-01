# Hands-On Lab — RBAC and Ownership Checks

## Objective

This lab applies role-based authorization and ownership enforcement to the Cardiac Patient Monitoring API already built in the project. The API uses ASP.NET Core Identity with the domain model's `Patient` entity, JWT bearer authentication, and EF Core. The goal is to ensure:

- every new registered user is assigned the `Patient` role by default,
- an initial `Admin` user is seeded safely and idempotently,
- the API authorization matches the real business purpose of each endpoint,
- authenticated patients can only access their own patient-scoped data,
- admin-only endpoints reject patient-role tokens with `403 Forbidden`.

## Starting Project State

Before this work, the project already had:

- `ApplicationUser : IdentityUser` with a one-to-one relationship to `Patient`
- a database context with seeded `Admin` and `Patient` roles
- JWT authentication using the `Jwt` configuration section
- an existing registration flow that created an `ApplicationUser` and a linked `Patient`
- login logic that returned a JWT containing the linked `Patient.Id` in the `patient_id` claim
- controller-level `[Authorize]` protection on most API endpoints
- no role-based policy enforcement for admin-only access
- no ownership verification for patient-specific resource access

## 1. Customer Role Assignment

### Existing Registration Flow

`POST /api/auth/register` is handled by `AuthController.Register` and `AuthService.RegisterAsync`.

The current flow:

1. Validates the profile details.
2. Creates the Identity user using `UserManager<ApplicationUser>`.
3. Creates the linked `Patient` record with the resulting `UserId`.
4. Returns an `AuthResponseDto` containing the JWT and `PatientId`.

### Customer Role

The project does not define a `Customer` role; the actual Identity role name in the code is `Patient`. The role is seeded in `ApplicationDbContext.OnModelCreating` as `IdentityRole { Name = "Patient" }`, and registration assigns that role to newly created users.

### Implementation

The real assignment happens in `AuthService.RegisterAsync`:

- `await EnsureRoleExistsAsync("Patient")` ensures the `Patient` role exists before assignment.
- `await users.AddToRoleAsync(u, "Patient")` assigns the role to the newly created user.
- the role is added before the linked `Patient` entity is saved in the same create flow.

This preserves the existing creation transaction and avoids allowing a user to self-register as `Admin` through the normal registration endpoint.

### Verification

New users receive the `Patient` role in their JWT through `TokenAsync`, which adds `ClaimTypes.Role` entries from `UserManager.GetRolesAsync(u)`. This is verified in the integration test `RegisterThenLogin_CreatesLinkedPatientAndJwtClaim`.

## 2. Initial Admin Seeding

### Admin Role

The project already seeds the `Admin` role in `ApplicationDbContext.OnModelCreating` using `IdentityRole { Name = "Admin" }`.

### Admin Account

The application now seeds an initial admin during startup in `Program.cs`.

The logic:

- ensures `Admin` and `Patient` roles exist,
- reads `InitialAdmin:Email` and `InitialAdmin:Password` from configuration,
- creates the admin user only if the email is not already present,
- adds the `Admin` role if it is missing,
- avoids duplicate users because the code checks `FindByEmailAsync` before creating.

### Seed Location

The role and admin seed logic is in:

- `CardiacPatientMonitoring.Api/Data/ApplicationDbContext.cs`
- `CardiacPatientMonitoring.Api/Program.cs`

### Idempotency

The admin seed is idempotent because it:

- checks `RoleManager.RoleExistsAsync(roleName)` before creating roles,
- checks `UserManager.FindByEmailAsync(adminEmail)` before creating the admin user,
- checks `UserManager.IsInRoleAsync(existingAdmin, "Admin")` before assigning the admin role.

### Configuration/Security

The project uses configuration rather than hard-coded secrets. The values are read from:

- `InitialAdmin:Email`
- `InitialAdmin:Password`

The repository file `appsettings.json` contains empty placeholders rather than real credentials, so no secret is exposed in source control.

### Verification

The app startup seeding is executed during host startup. The configuration remains safe because no actual admin password or token is written to documentation or committed to source control.

## 3. Endpoint Authorization Audit

The API endpoint inventory after the fix is:

| Method | Route | Controller | Access Level | Authorization |
| --- | --- | --- | --- | --- |
| POST | `/api/auth/register` | `AuthController` | Public | `[AllowAnonymous]` |
| POST | `/api/auth/login` | `AuthController` | Public | `[AllowAnonymous]` |
| GET | `/api/patients` | `PatientsController` | Admin-only | `[Authorize(Roles = "Admin")]` |
| GET | `/api/patients/{id:int}` | `PatientsController` | Authenticated user, with ownership check | `[Authorize]` + patient ID match |
| POST | `/api/patients` | `PatientsController` | Admin-only | `[Authorize(Roles = "Admin")]` |
| PUT | `/api/patients/{id:int}` | `PatientsController` | Admin-only | `[Authorize(Roles = "Admin")]` |
| DELETE | `/api/patients/{id:int}` | `PatientsController` | Admin-only | `[Authorize(Roles = "Admin")]` |
| GET | `/api/patients/{patientId:int}/vital-signs` | `VitalSignsController` | Authenticated patient or Admin | `[Authorize]` + patient ownership check |
| GET | `/api/vital-signs/{id:int}` | `VitalSignsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| POST | `/api/patients/{patientId:int}/vital-signs` | `VitalSignsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| PUT | `/api/vital-signs/{id:int}` | `VitalSignsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| DELETE | `/api/vital-signs/{id:int}` | `VitalSignsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| GET | `/api/patients/{patientId:int}/medications` | `MedicationsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| GET | `/api/medications/{id:int}` | `MedicationsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| POST | `/api/patients/{patientId:int}/medications` | `MedicationsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| PUT | `/api/medications/{id:int}` | `MedicationsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| DELETE | `/api/medications/{id:int}` | `MedicationsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| GET | `/api/patients/{patientId:int}/appointments` | `AppointmentsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| GET | `/api/appointments/{id:int}` | `AppointmentsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| POST | `/api/patients/{patientId:int}/appointments` | `AppointmentsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| PUT | `/api/appointments/{id:int}` | `AppointmentsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| DELETE | `/api/appointments/{id:int}` | `AppointmentsController` | Authenticated patient or Admin | `[Authorize]` + ownership check |
| GET | `/api/test/exception` | `TestController` | Public | `[AllowAnonymous]` for middleware exception testing |

## 4. RBAC Implementation

The project uses the actual ASP.NET Core Identity role system with JWT role claims. Authentication is configured in `Program.cs` with:

- `AddIdentityCore<ApplicationUser>()`
- `.AddRoles<IdentityRole>()`
- `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`
- `TokenValidationParameters.RoleClaimType = ClaimTypes.Role`

Authorization is configured with policies, including:

- `AdminOnly` and `PatientOnly` policies from `AddAuthorization(options => ...)`
- route-level `[Authorize(Roles = "Admin")]` usage for admin endpoints
- route-level `[Authorize]` for authenticated endpoints with additional patient ownership checks

The role resolution is done through the JWT claim `ClaimTypes.Role` generated from `UserManager.GetRolesAsync(u)`.

## 5. Ownership Checks

### Ownership Model

The ownership owner in this project is the `Patient` entity. Each record (`VitalSign`, `Medication`, `Appointment`) has a `PatientId` and each `ApplicationUser` is linked to exactly one `Patient` through `Patient.UserId`.

### JWT Claim Used

Ownership is based on the `patient_id` claim, which is generated in `AuthService.TokenAsync` and matches the user’s linked `Patient.Id`.

### Protected Endpoints

The following endpoints enforce patient ownership:

- `GET /api/patients/{id:int}`
- `GET /api/patients/{patientId:int}/vital-signs`
- `GET /api/vital-signs/{id:int}`
- `POST /api/patients/{patientId:int}/vital-signs`
- `PUT /api/vital-signs/{id:int}`
- `DELETE /api/vital-signs/{id:int}`
- `GET /api/patients/{patientId:int}/medications`
- `GET /api/medications/{id:int}`
- `POST /api/patients/{patientId:int}/medications`
- `PUT /api/medications/{id:int}`
- `DELETE /api/medications/{id:int}`
- `GET /api/patients/{patientId:int}/appointments`
- `GET /api/appointments/{id:int}`
- `POST /api/patients/{patientId:int}/appointments`
- `PUT /api/appointments/{id:int}`
- `DELETE /api/appointments/{id:int}`

### Ownership Validation

The control flow is:

- authenticated user token is validated,
- JWT claim `patient_id` is read,
- the requested resource is loaded,
- the resource’s `PatientId` is compared to the authenticated patient ID,
- admins bypass the patient-specific owner check,
- non-admin users receive `403 Forbidden` when the patient ID does not match.

### Unauthorized Ownership Access

The logic uses `Forbid()` when the resource exists but belongs to a different patient. This is the project’s actual behavior after the fix.

## 6. Security Testing

### Customer → Admin Endpoint Test #1

- Endpoint: `GET /api/patients`
- Token role: `Patient`
- Expected: `403 Forbidden`
- Actual: `403 Forbidden`

### Customer → Admin Endpoint Test #2

- Endpoint: `POST /api/patients`
- Token role: `Patient`
- Expected: `403 Forbidden`
- Actual: `403 Forbidden`

### Customer A → Customer B Resource Test

- Customer A: unique registered patient account
- Customer B: second registered patient account
- Resource: `/api/patients/{customerA.patientId}`
- Request by: Customer B with a valid `Patient` JWT
- Expected: `403 Forbidden`
- Actual: `403 Forbidden`

The automated regression tests for this behavior are in `CardiacPatientMonitoring.Tests/AuthEndpointIntegrationTests.cs`.

## 7. Database / EF Core Changes

No migration was required for the RBAC and ownership work because the schema already contained:

- the Identity roles table,
- the Identity user table,
- the `Patient` entity with `UserId`,
- the `PatientId` relationships on `VitalSign`, `Medication`, and `Appointment`.

The project already had the necessary database shape in the current model, and the new behavior is implemented in code only.

## 8. Validation Results

The actual verification run was:

- `dotnet test CardiacPatientMonitoring.slnx --no-restore`

Result:

- build succeeded,
- all 33 automated tests passed,
- RBAC checks passed,
- ownership checks passed,
- no migration was needed.

## 9. Problems Encountered

No production blockers remained after the final fix. The only warning during the build was `NU1900` from NuGet vulnerability metadata lookup failing because the machine could not reach the nuget service index. This did not block the build or tests.

## 10. Security Considerations

- The application does not allow unrestricted role selection during registration.
- The `Patient` role is assigned by the server, not by user input.
- The `patient_id` claim is used as the authoritative domain identity.
- `Forbid()` is used for ownership mismatches rather than trusting the request path alone.
- The admin seed is safely configured via environment or appsettings placeholders, without hard-coded credentials.

## 11. Final Result

The Day 3 implementation is complete:

- new users receive the `Patient` role by default,
- the initial `Admin` account is seeded idempotently,
- admin-only API endpoints are protected by role checks,
- patient-specific data is guarded by ownership validation,
- the project’s automated tests pass.
