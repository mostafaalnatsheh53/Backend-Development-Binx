# Day 4 — Authentication, RBAC & Middleware

## Overview

Day 4 consolidates the Cardiac Patient Monitoring API security work and adds a custom request-correlation middleware. The project contains Identity-backed registration and login, JWT bearer authentication, role-based authorization, patient ownership checks, centralized exception handling, automated endpoint tests, and the completed middleware lab documentation.

## Objectives

- Authenticate users with Identity and JWT bearer tokens.
- Enforce role-based and ownership-based authorization.
- Identify and implement a real cross-cutting concern with custom middleware.
- Verify behavior across multiple API endpoints.
- Prepare a focused Sprint 2 commit and Pull Request description.

## Authentication

Authentication uses ASP.NET Core Identity with Entity Framework Core persistence. `AuthService` registers patients, assigns the `Patient` role, links the patient record to the Identity user, and issues a JWT on registration and login.

JWT configuration is in `CardiacPatientMonitoring.Api/appsettings.json`: issuer, audience, signing key, and a 120-minute expiration. Token validation checks issuer, audience, lifetime, signing key, and maps `ClaimTypes.Role` to roles. Tokens include the Identity user ID, email, `patient_id`, and assigned roles.

`/api/auth/register` and `/api/auth/login` are anonymous endpoints. Patient, vital-sign, medication, and appointment endpoints are protected with `[Authorize]`; patient administration endpoints additionally require the `Admin` role.

## RBAC / Authorization

The implemented roles are `Admin` and `Patient`. Policies named `AdminOnly` and `PatientOnly` are registered, while controllers currently use `[Authorize]` and `[Authorize(Roles = "Admin")]` attributes directly.

Patients may access their own patient-linked resources. Admins may access the administrative patient routes and bypass ownership checks. A missing JWT results in HTTP 401. A valid user without the required role, or a patient attempting to access another patient’s resource, results in HTTP 403.

## Middleware

The Day 4 custom middleware addresses request correlation. `CorrelationIdMiddleware` accepts a safe `X-Correlation-ID` request header or generates a new ID, assigns it to `HttpContext.TraceIdentifier`, and returns it in the response header. This applies centrally to all endpoints, including authorization failures and handled exceptions.

It is registered in `Program.cs` after `ExceptionHandlingMiddleware` and before HTTPS redirection, authentication, authorization, and mapped controllers. No controller changes are needed for the concern.

## Request Pipeline

After application startup and Identity role initialization, the request pipeline is:

1. `ExceptionHandlingMiddleware` catches known service exceptions and unexpected exceptions and returns problem details.
2. `CorrelationIdMiddleware` establishes or propagates `X-Correlation-ID`.
3. HTTPS redirection runs when applicable.
4. `UseAuthentication` validates the bearer token and builds the authenticated identity.
5. `UseAuthorization` evaluates `[Authorize]`, role requirements, and policies.
6. `MapControllers` dispatches authorized requests to controller endpoints.

OpenAPI and Swagger are enabled only in Development. The application uses conventional controller endpoint routing through `MapControllers`.

## Hands-On Lab

### Cross-Cutting Concern

Request correlation was selected because clients and server logs need a shared request identifier across many endpoints. The existing application did not expose one; custom middleware centralizes the behavior.

### Custom Middleware

`CardiacPatientMonitoring.Api/Middleware/CorrelationIdMiddleware.cs` validates incoming IDs, generates missing IDs, sets the trace identifier, and adds the response header. It runs before authentication and authorization.

### Endpoint Verification

Automated integration tests verify the header on invalid registration, unauthenticated patient listing, and the controlled exception endpoint. Manual/Postman verification was not performed in the current environment.

### Authentication & RBAC Verification

Existing tests verify registration and login JWT claims, rejection of unauthenticated requests, Admin-only endpoint restrictions, and patient ownership restrictions.

### Pull Request

The active branch is `feature/week07-day3-rbac-ownership`. The intended title is `feat(week07): complete Day 4 middleware lab`. A Pull Request link, remote push result, and reviewer are not verified in the current environment.

### Mentor Review

No mentor feedback was found in the Day 4 project or available Git history. Mentor review is not verified in the current environment.

## Testing & Verification

- Existing baseline: 33 tests passed before the middleware change.
- Middleware integration tests: 3 passed, covering anonymous registration, protected patient listing, and exception handling.
- Final full run: 36 tests passed, 0 failed, 0 skipped.
- Authentication tests cover registration, login, JWT claims, and missing credentials.
- RBAC tests cover Admin-only restrictions and cross-patient ownership restrictions.
- Controller and service tests cover endpoint results and exception mapping.
- Manual/Postman verification: Not verified in the current environment.

The test build reports the existing `NU1900` warning because the NuGet vulnerability service could not be reached. It does not cause test failure.

## Git & Pull Request

- **Branch:** `feature/week07-day3-rbac-ownership`.
- **Relevant history:** This project is based on the Week 07 authentication and RBAC work already present on the active branch.
- **Commit:** Recorded after the final validation run.
- **Pull Request:** Not verified in the current environment; no link is claimed.
- **Reviewer:** Not verified in the current environment.
- **Mentor feedback:** None found in the project or available Git history.

## Files Changed

- `CardiacPatientMonitoring.Api/Middleware/CorrelationIdMiddleware.cs`: new request-correlation middleware.
- `CardiacPatientMonitoring.Api/Program.cs`: middleware registration.
- `CardiacPatientMonitoring.Tests/CorrelationIdMiddlewareIntegrationTests.cs`: endpoint integration coverage.
- `hand-on-lab.md`: complete Hands-On Lab record.
- `README.md`: complete Day 4 documentation.

## What I Learned

- Identity manages users and roles, while JWT carries authenticated claims between requests.
- RBAC expresses role requirements; authorization also needs ownership checks for patient data.
- Middleware handles cross-cutting behavior at the request boundary without duplicating controller logic.
- Pipeline order determines whether correlation, authentication, authorization, and exception handling observe the same request.
- Automated integration tests provide repeatable verification across endpoint boundaries.
- A Pull Request should state the security behavior, middleware change, tests, documentation, and review state without claiming unverifiable results.

## Final Status

| Area | Status |
| --- | --- |
| Authentication | Implemented and covered by integration tests; 33 baseline tests passed. |
| RBAC | Implemented for `Admin` and `Patient`, including ownership checks; covered by integration tests. |
| Custom Middleware | Implemented and registered: request correlation via `X-Correlation-ID`. |
| Endpoint Verification | Automated verification passed for auth, protected patient, and exception endpoints. Manual/Postman verification is not verified in the current environment. |
| Automated Tests | Final full run passed 36/36; focused middleware run passed 3/3. |
| Sprint 2 Branch | Active branch: `feature/week07-day3-rbac-ownership`. |
| Pull Request | Not created or link-verified in the current environment. |
| Mentor Review | Not verified in the current environment. |

Remaining manual steps: run Postman if desired, push the final commit, create the Pull Request, and update this documentation with the actual PR link, reviewer, and mentor feedback.