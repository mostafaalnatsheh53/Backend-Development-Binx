# Day 5 — Sprint 2 Close-Out

## Overview

This Day 5 review verifies the completed Sprint 2 authentication and authorization work for the Cardiac Patient Monitoring API. The project uses ASP.NET Core Identity with JWT-based authentication, role-based authorization, and patient ownership checks for protected resources.

## Objectives

- Confirm the registration and login flow.
- Verify JWT creation, validation, and authenticated requests.
- Confirm role-based access control (RBAC) behavior.
- Review protected endpoints and ownership enforcement.
- Validate the Sprint 2 backlog and summarize remaining work.
- Document Day 5 status and the Sprint 3 follow-up items.

## Sprint 2 Authentication

### Registration

The API exposes `POST /api/auth/register` in `AuthController` and accepts `RegisterDto` values for email, password, first name, last name, date of birth, gender, and optional phone number. The `AuthService.RegisterAsync` method validates the date of birth, ensures the `Patient` role exists, creates the Identity user, assigns the `Patient` role, creates the linked `Patient` record, and returns a JWT plus the generated patient ID.

### Login

The API exposes `POST /api/auth/login` in `AuthController`. The `AuthService.LoginAsync` method looks up the user by email, validates the password, confirms the associated patient record exists, and then returns a JWT and patient ID.

### JWT/token generation

JWT settings are configured in `appsettings.json`:

- Issuer: `CardiacPatientMonitoring`
- Audience: `CardiacPatientMonitoringClient`
- Expiration: 120 minutes
- Signing key: configured in the `Jwt` section

The token includes:

- `ClaimTypes.NameIdentifier` = user ID
- `ClaimTypes.Email` = email
- `patient_id` = patient ID
- `ClaimTypes.Role` = assigned role(s)

### Token validation

`Program.cs` configures JWT bearer authentication with:

- `ValidateIssuer = true`
- `ValidateAudience = true`
- `ValidateLifetime = true`
- `ValidateIssuerSigningKey = true`
- `RoleClaimType = ClaimTypes.Role`
- `NameClaimType = ClaimTypes.NameIdentifier`

### Authenticated requests

After login, requests include the JWT in the `Authorization: Bearer <token>` header. Protected endpoints use `[Authorize]` or `[Authorize(Roles = "Admin")]`.

## RBAC & Authorization

### Roles

The application defines and seeds two roles:

- `Admin`
- `Patient`

### Role-based authorization

`Program.cs` registers the roles with ASP.NET Core Identity and adds authorization policies for:

- `AdminOnly` => `RequireRole("Admin")`
- `PatientOnly` => `RequireRole("Patient")`

The controllers currently enforce the authorization rules directly using `[Authorize]`, `[Authorize(Roles = "Admin")]`, and runtime ownership checks.

### Protected endpoints

Protected endpoints include:

- `GET /api/patients` — requires `Admin`
- `GET /api/patients/{id}` — `Admin` or same patient
- `POST /api/patients` — requires `Admin`
- `PUT /api/patients/{id}` — requires `Admin`
- `DELETE /api/patients/{id}` — requires `Admin`
- `GET /api/patients/{patientId}/vital-signs` — authenticated same patient or `Admin`
- `GET /api/patients/{patientId}/medications` — authenticated same patient or `Admin`
- `GET /api/patients/{patientId}/appointments` — authenticated same patient or `Admin`

### Authorization rules

- Admins can access all patient resources.
- Patients can access only their own patient resource and nested resources when `patient_id` matches the requested patient ID.
- Unauthenticated requests are rejected by the JWT middleware.

## RBAC Matrix

| Role | Resource/Endpoint | Access |
|------|-------------------|--------|
| Admin | `GET /api/patients` | Allowed |
| Admin | `GET /api/patients/{id}` | Allowed |
| Admin | `POST /api/patients` | Allowed |
| Admin | `PUT /api/patients/{id}` | Allowed |
| Admin | `DELETE /api/patients/{id}` | Allowed |
| Patient | `GET /api/patients` | Denied |
| Patient | `GET /api/patients/{id}` | Allowed only if `patient_id` matches |
| Patient | `POST /api/patients` | Denied |
| Patient | `GET /api/patients/{patientId}/vital-signs` | Allowed only if `patient_id` matches |
| Patient | `GET /api/patients/{patientId}/medications` | Allowed only if `patient_id` matches |
| Patient | `GET /api/patients/{patientId}/appointments` | Allowed only if `patient_id` matches |
| Anonymous | Any protected endpoint | Denied |

## Authentication & RBAC Flow

Registration
→ Login
→ JWT token generation
→ Bearer token in Authorization header
→ Authentication middleware validation
→ Authorization rules evaluated
→ Protected endpoint access granted or denied

## Hands-On Lab

The Sprint 2 close-out lab requires verification of the authentication/RBAC flow, rejection cases, backlog status, and final summary. The project was inspected and validated against the implemented code, seeded roles, and integration tests.

### Authentication & RBAC Demo

The project includes a complete demo flow for user registration and login, returning a JWT that is then used for subsequent access control checks. The role claim and patient ownership claim are validated as part of the token.

### Deliberate Rejection Cases

1. Unauthenticated request to `GET /api/patients` -> `401 Unauthorized`.
2. Authenticated patient tries to access another patient record -> `403 Forbidden`.

### Backlog Review

No explicit Sprint 2 backlog file was found in this project directory. The implementation was therefore reviewed using the actual code, seeded roles, and integration tests rather than a separate task list artifact.

### Sprint 3 Backlog

The only actionable item discovered from implementation is an authorization bootstrap gap: the application seeds roles but requires `InitialAdmin.Email` and `InitialAdmin.Password` to be set before an actual `Admin` user exists.

### Authorization Edge Cases

The main unresolved issue is the configuration-dependent admin bootstrap. Without configured admin credentials, the `Admin` role exists but no usable admin account is created.

### Sprint 2 Retrospective

See the retrospective section later in this document.

### Sprint 2 Summary

The Day 5 review confirms that the main Sprint 2 authentication and RBAC work is implemented and verified by tests. Key details are described in the summary below.

## Testing & Verification

### Registration verification

Verified by the integration test `RegisterThenLogin_CreatesLinkedPatientAndJwtClaim`, which creates a patient, checks the returned `PatientId`, confirms a linked Identity user and patient record, and validates the JWT claims.

### Login verification

Verified by the same integration test and the login request flow. The login response returns a JWT and a valid patient ID.

### Authentication verification

Verified by `PatientEndpointIntegrationTests.GetPatients_WithoutJwt_ReturnsUnauthorized` and the JWT validation in `Program.cs`.

### RBAC verification

Verified by `CustomerToken_IsRejectedFromAdminOnlyEndpoints` and `CustomerB_CannotFetchCustomerAResource`.

### Authorization verification

Verified by the runtime ownership checks in the patient, vital sign, medication, and appointment controllers.

### Rejection cases

The following rejection cases were confirmed in tests:

- `401 Unauthorized` for missing token on a protected endpoint.
- `403 Forbidden` for a patient accessing another patient’s record.

### Automated tests

Command run:

`dotnet test "CardiacPatientMonitoring.slnx" --nologo`

Result:

- Total: 36
- Failed: 0
- Succeeded: 36
- Skipped: 0

### Manual/Postman verification

Not verified in the current environment.

## Git & Pull Request

### Sprint 2 branch

The repository is currently on `main` in the local clone.

### Relevant commits

Current local HEAD:

- `7d2b08e` — `feat: implement middleware and document Day 4`

### Pull Request title

Not verifiable from the current repository state.

### Pull Request summary

Not verifiable from the current repository state.

### Pull Request link

Not verifiable from the current repository state.

### Merge status

Not verifiable from the current repository state.

### Mentor reviewer

Not verifiable from the current repository state.

### Feedback addressed

No mentor feedback or PR review information was available in the current environment.

## Sprint 3 Backlog

### [AUTHORIZATION-EDGE-CASE] Admin bootstrap is configuration-dependent

- Task: Ensure an actual admin account can be created or seeded reliably.
- Context: `Program.cs` seeds the `Admin` role and checks `InitialAdmin:Email` and `InitialAdmin:Password`, but the default values in `appsettings.json` are empty.
- Why it remains incomplete: The `Admin` role exists, but there is no usable admin account unless the environment is configured manually.
- Acceptance criteria / expected outcome: A valid admin user exists and can log in successfully; `AdminOnly` endpoints work without manual code changes.
- Suggested next action: Configure valid `InitialAdmin` values or implement a dedicated bootstrap script for production and testing environments.

No other explicit Sprint 2 backlog items were present in this project directory.

## Sprint 2 Retrospective

### What Went Well

- Registration and login both work and return a JWT.
- The JWT includes the role and patient claims needed for authorization checks.
- Protected endpoints enforce `Admin` and patient ownership rules.
- The integration test suite validates the main auth and RBAC flows.

### What Didn't Go Well

- There was no explicit Sprint 2 backlog artifact in this project directory to review against.
- The admin bootstrap relies on configuration values that are empty by default.
- Manual Postman verification was not performed in the current environment.

### Sprint 3 Action

Create a verified admin bootstrap workflow that guarantees an admin account exists in every environment before relying on `Admin`-only endpoints. This will be validated by a login test that hits a protected admin endpoint with a seeded admin user.

## Files Changed

The Day 5 work created the following documentation files in the project directory:

- `README.md` — full Day 5 summary and verification record.
- `hand-on-lab.md` — Sprint 2 close-out hands-on lab write-up.

No application source files were modified during Day 5 documentation work.

## What I Learned

- Authentication is implemented through ASP.NET Core Identity + JWT bearer middleware.
- Registration creates a user, assigns a role, and links a patient record.
- Login returns the JWT with the patient and role claims needed for protected access.
- RBAC is enforced by `[Authorize]` and ownership-based checks.
- Rejection handling is implemented as `401 Unauthorized` and `403 Forbidden` responses.
- Backlog review should be evidence-based and should not invent tasks.
- Pull requests and mentor review require verifiable repo state and remote metadata.

## Final Status

| Area | Status |
|------|--------|
| Registration | Verified |
| Login | Verified |
| Authentication | Verified |
| RBAC | Verified |
| Authorization | Verified |
| Rejection Cases | Verified |
| Sprint 2 Backlog Review | Completed with no explicit backlog artifact found |
| Sprint 3 Backlog | Documented |
| Retrospective | Completed |
| Pull Request | Not verifiable from current environment |
| Mentor Review | Not verifiable from current environment |
