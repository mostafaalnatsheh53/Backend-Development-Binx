# Week 07 — Day 3

## RBAC and Ownership Checks

### Day Objective

Apply role-based authorization and ownership validation to the existing Cardiac Patient Monitoring API. The goal is to protect the application based on the actual Identity roles and the linked `Patient` entity, not by trusting request data alone.

### Project

This project is the Day 3 continuation of the Cardiac Patient Monitoring API within the internship workspace. It uses:

- ASP.NET Core Web API
- ASP.NET Core Identity
- EF Core with SQL Server in normal environments
- JWT bearer authentication
- a domain model with `Patient`, `VitalSign`, `Medication`, and `Appointment`

### Starting Point

At the start of Day 3, the API had already implemented:

- `ApplicationUser : IdentityUser`
- a required `Patient.UserId` relationship to the user
- registration and login endpoints
- a JWT containing the linked `Patient.Id` in the `patient_id` claim
- controller-level `[Authorize]` annotations on most routes

The missing pieces were RBAC for admin-only endpoints and ownership checks for patient-scoped data.

### Day Plan

1. Inspect the existing Identity, roles, claims, and controllers.
2. Add default `Patient` role assignment during registration.
3. Seed an initial `Admin` user safely and idempotently.
4. Audit every endpoint by actual business purpose.
5. Apply role-based access requirements.
6. Enforce ownership checks using the JWT `patient_id` claim.
7. Validate with automated tests and document the final result.

### 1. Authentication and Authorization Architecture

The real implementation uses:

- `ApplicationUser` as the Identity user type
- `Patient` as the user’s linked domain entity
- `IdentityRole` for the roles `Admin` and `Patient`
- JWT bearer token validation configured in `Program.cs`
- `ClaimTypes.NameIdentifier`, `ClaimTypes.Email`, and `ClaimTypes.Role`
- the custom claim `patient_id` for the authenticated patient record ID

Role handling is configured by `AddIdentityCore<ApplicationUser>()` and `.AddRoles<IdentityRole>()`, and the JWT validation includes:

- `NameClaimType = ClaimTypes.NameIdentifier`
- `RoleClaimType = ClaimTypes.Role`

The service layer builds tokens in `AuthService.TokenAsync` and adds all role claims using `UserManager.GetRolesAsync(u)`.

### 2. Customer Role

The project’s actual user-facing domain entity is `Patient`, and the actual Identity role assigned during registration is `Patient`.

Registration now does the following:

- ensures the `Patient` role exists,
- creates the `ApplicationUser`,
- adds the new user to the `Patient` role,
- creates the linked `Patient` record,
- returns the JWT containing the `patient_id` claim and role claims.

### 3. Initial Admin Seed

The app seeds the admin account during startup in `Program.cs`.

It:

- ensures the `Admin` role exists,
- ensures the `Patient` role exists,
- reads `InitialAdmin:Email` and `InitialAdmin:Password` from configuration,
- creates the admin user only if it does not exist,
- assigns the `Admin` role if required,
- remains idempotent across repeated startup runs.

This avoids hard-coding secrets in source control; placeholders are used in configuration.

### 4. Endpoint Authorization Matrix

| Method | Route | Access Level |
| --- | --- | --- |
| POST | `/api/auth/register` | Public |
| POST | `/api/auth/login` | Public |
| GET | `/api/patients` | Admin-only |
| GET | `/api/patients/{id:int}` | Authenticated patient or Admin |
| POST | `/api/patients` | Admin-only |
| PUT | `/api/patients/{id:int}` | Admin-only |
| DELETE | `/api/patients/{id:int}` | Admin-only |
| GET | `/api/patients/{patientId:int}/vital-signs` | Authenticated patient or Admin |
| GET | `/api/vital-signs/{id:int}` | Authenticated patient or Admin |
| POST | `/api/patients/{patientId:int}/vital-signs` | Authenticated patient or Admin |
| PUT | `/api/vital-signs/{id:int}` | Authenticated patient or Admin |
| DELETE | `/api/vital-signs/{id:int}` | Authenticated patient or Admin |
| GET | `/api/patients/{patientId:int}/medications` | Authenticated patient or Admin |
| GET | `/api/medications/{id:int}` | Authenticated patient or Admin |
| POST | `/api/patients/{patientId:int}/medications` | Authenticated patient or Admin |
| PUT | `/api/medications/{id:int}` | Authenticated patient or Admin |
| DELETE | `/api/medications/{id:int}` | Authenticated patient or Admin |
| GET | `/api/patients/{patientId:int}/appointments` | Authenticated patient or Admin |
| GET | `/api/appointments/{id:int}` | Authenticated patient or Admin |
| POST | `/api/patients/{patientId:int}/appointments` | Authenticated patient or Admin |
| PUT | `/api/appointments/{id:int}` | Authenticated patient or Admin |
| DELETE | `/api/appointments/{id:int}` | Authenticated patient or Admin |
| GET | `/api/test/exception` | Public (middleware test route) |

### 5. RBAC Implementation

Admin-only enforcement is applied to the actual administrative endpoints using `[Authorize(Roles = "Admin")]`.

Authenticated patient-specific routes use `[Authorize]` and then compare the current JWT `patient_id` with the target resource owner.

The role checks are backed by the JWT role claim: `ClaimTypes.Role`.

### 6. Ownership Implementation

The project uses the `Patient` entity as the owner. Each owner-scoped resource has a `PatientId` and the JWT contains the authenticated patient’s ID in the `patient_id` claim.

The actual flow is:

```text
Customer Request
→ JWT Authentication
→ Read Domain Entity ID Claim (`patient_id`)
→ Load Resource
→ Compare Resource Owner (`PatientId`)
→ Allow / Reject
```

If the current patient does not match the resource owner, the endpoint returns `403 Forbidden`.

### 7. Security Testing

The automated tests confirm:

- a `Patient` token is rejected from `GET /api/patients` with `403 Forbidden`
- a `Patient` token is rejected from `POST /api/patients` with `403 Forbidden`
- a `Patient` token cannot fetch another patient’s resource with `403 Forbidden`

### 8. Database Changes

No schema migration was required. The existing Identity and patient relationship tables already supported the RBAC and ownership work.

### 9. API Endpoint Matrix

MethodRouteAccessOwnership

- POST `/api/auth/register` — Public — N/A
- POST `/api/auth/login` — Public — N/A
- GET `/api/patients` — Admin-only — N/A
- GET `/api/patients/{id:int}` — Authenticated patient or Admin — owner match required
- POST `/api/patients` — Admin-only — N/A
- PUT `/api/patients/{id:int}` — Admin-only — N/A
- DELETE `/api/patients/{id:int}` — Admin-only — N/A
- GET `/api/patients/{patientId:int}/vital-signs` — Authenticated patient or Admin — patient ownership
- GET `/api/vital-signs/{id:int}` — Authenticated patient or Admin — patient ownership
- POST `/api/patients/{patientId:int}/vital-signs` — Authenticated patient or Admin — patient ownership
- PUT `/api/vital-signs/{id:int}` — Authenticated patient or Admin — patient ownership
- DELETE `/api/vital-signs/{id:int}` — Authenticated patient or Admin — patient ownership
- GET `/api/patients/{patientId:int}/medications` — Authenticated patient or Admin — patient ownership
- GET `/api/medications/{id:int}` — Authenticated patient or Admin — patient ownership
- POST `/api/patients/{patientId:int}/medications` — Authenticated patient or Admin — patient ownership
- PUT `/api/medications/{id:int}` — Authenticated patient or Admin — patient ownership
- DELETE `/api/medications/{id:int}` — Authenticated patient or Admin — patient ownership
- GET `/api/patients/{patientId:int}/appointments` — Authenticated patient or Admin — patient ownership
- GET `/api/appointments/{id:int}` — Authenticated patient or Admin — patient ownership
- POST `/api/patients/{patientId:int}/appointments` — Authenticated patient or Admin — patient ownership
- PUT `/api/appointments/{id:int}` — Authenticated patient or Admin — patient ownership
- DELETE `/api/appointments/{id:int}` — Authenticated patient or Admin — patient ownership

### 10. Validation

Validated with the real project command:

```bash
dotnet test CardiacPatientMonitoring.slnx --no-restore
```

Result: 33 tests passed, 0 failed.

### 11. Challenges and Solutions

The main challenge was ensuring the API did not trust a client-supplied patient ID. The fix was to rely on the JWT claim `patient_id`, which is derived from the authenticated user’s linked `Patient` record. This preserves the system’s domain security model while keeping the API simple.

### 12. Security Considerations

- server-side role assignment only,
- admin credentials are configuration-driven,
- JWT claims represent the domain identity, not request parameters,
- cross-patient access is blocked with `403 Forbidden`,
- no secrets were committed to the repository.

### 13. Git

Branch: `feature/week07-day3-rbac-ownership`

Commit message used when committing this implementation:

```text
feat(auth): apply RBAC and ownership checks
```

### 14. Final Day Outcome

The API now applies the actual role model and ownership rules to the domain, prevents cross-patient access, and allows only the correct user/admin roles to reach each endpoint.

### 15. Files Changed

- `CardiacPatientMonitoring.Api/Program.cs`
- `CardiacPatientMonitoring.Api/Services/Services.cs`
- `CardiacPatientMonitoring.Api/Controllers/Controllers.cs`
- `CardiacPatientMonitoring.Api/appsettings.json`
- `CardiacPatientMonitoring.Tests/AuthEndpointIntegrationTests.cs`
- `CardiacPatientMonitoring.Tests/PatientEndpointIntegrationTests.cs`
- `HAND-ON-LAB.md`
- `README.md`
