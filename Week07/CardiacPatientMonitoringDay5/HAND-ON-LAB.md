# Hands-On Lab — Sprint 2 Close-Out

## Objective

The Sprint 2 close-out verifies the completed authentication, authorization, and RBAC work for the Cardiac Patient Monitoring API. The objective is to validate the registration flow, JWT-based authentication, protected endpoint access, and the remaining follow-up work using the actual implementation and tests.

## 1. Authentication & RBAC Demo

The flow is implemented as follows:

Registration → Login → JWT token → Authorization header → Authentication middleware → Authorization rules → Protected endpoint access.

Verified implementation details:

- `POST /api/auth/register` creates the Identity user, assigns the `Patient` role, creates a linked patient row, and returns a JWT and patient ID.
- `POST /api/auth/login` validates credentials, returns a JWT, and includes the patient ID and role claims.
- JWT validation is configured in `Program.cs`.
- `PatientsController` and the nested resource controllers enforce role checks and patient ownership rules.
- Role claims are mapped to `ClaimTypes.Role`, and `patient_id` is used to enforce same-patient access.

## 2. Deliberate Rejection Cases

### Case 1: Unauthenticated user calling a protected endpoint

- Scenario: Request `/api/patients` without a token.
- Endpoint: `GET /api/patients`
- User/role: Anonymous user
- Expected result: `401 Unauthorized`
- Actual result: `401 Unauthorized`
- Verification method: Integration test `GetPatients_WithoutJwt_ReturnsUnauthorized`

### Case 2: Authenticated patient accessing another patient record

- Scenario: A patient logs in and requests another patient’s details.
- Endpoint: `GET /api/patients/{otherPatientId}`
- User/role: `Patient` trying to access another patient’s profile
- Expected result: `403 Forbidden`
- Actual result: `403 Forbidden`
- Verification method: Integration test `CustomerB_CannotFetchCustomerAResource`

### Case 3: Patient without admin role calling an admin-only endpoint

- Scenario: A registered patient tries to list all patients.
- Endpoint: `GET /api/patients`
- User/role: `Patient`
- Expected result: `403 Forbidden`
- Actual result: `403 Forbidden`
- Verification method: Integration test `CustomerToken_IsRejectedFromAdminOnlyEndpoints`

> Manual/Postman verification was not performed in the current environment. The rejection cases listed above are based on the automated integration tests and actual code paths.

## 3. Sprint 2 Backlog Review

No explicit Sprint 2 backlog artifact was present in this project directory. The implementation was reviewed through the verified code and automated tests rather than a separate backlog file.

### Sprint 2 tasks reviewed

- Registration flow: Completed and verified.
- Login flow: Completed and verified.
- JWT generation and validation: Completed and verified.
- RBAC and ownership enforcement: Completed and verified.
- Protected endpoint enforcement: Completed and verified.

### Incomplete tasks

No explicit Sprint 2 backlog items were found in the project. The only actionable follow-up item identified from implementation data is the admin bootstrap gap.

### Tasks moved to Sprint 3

#### [AUTHORIZATION-EDGE-CASE] Admin bootstrap is configuration-dependent

- Task: Ensure a valid admin account exists in every environment.
- Context: `Program.cs` seeds the `Admin` role but depends on empty `InitialAdmin` settings in `appsettings.json` for real admin creation.
- Why it remains incomplete: The role exists, but there is no usable administrator unless the environment is configured manually.
- Acceptance criteria / expected outcome: An admin user can log in and access `AdminOnly` endpoints without manual code changes.
- Suggested next action: Configure valid admin bootstrap values or add a dedicated initialization routine.

## 4. Authorization Edge Cases

No broad authorization bug was found beyond the admin bootstrap gap.

### [AUTHORIZATION-EDGE-CASE] Admin bootstrap gap

- Edge case: No real `Admin` user exists unless `InitialAdmin` values are configured.
- Current behavior: The `Admin` role gets created, but no admin account is created automatically when credentials are blank.
- Expected behavior: A seeded or configured admin account should be available and able to access admin-protected endpoints.
- Why it matters: Admin-only access cannot be verified in a clean environment without additional configuration.
- Suggested action: Add a deterministic bootstrap path or configure valid initial admin credentials.

## 5. Sprint 2 Retrospective

### What Went Well

- Registration and login are working end-to-end.
- JWT generation and validation are correctly configured.
- RBAC rules and patient ownership checks are enforced for protected endpoints.
- Automated tests cover the major auth and authorization paths.

### What Didn't Go Well

- There was no explicit Sprint 2 backlog artifact in the project to track tasks directly.
- Admin bootstrap depends on environment configuration.
- Manual API verification was not run in the current environment.

### Sprint 3 Action

Create a reliable admin bootstrap workflow that guarantees an admin account exists in every environment and verify it through an admin login test and a protected admin endpoint call.

## 6. Sprint 2 Summary

The Sprint 2 work is implemented and validated for the main authentication and RBAC flows.

### Registration flow

- `POST /api/auth/register` accepts registration data.
- The Identity user is created.
- The `Patient` role is assigned.
- A linked patient record is created.
- A JWT and patient ID are returned.

### Login flow

- `POST /api/auth/login` validates the email and password.
- The user’s patient record is resolved.
- A JWT is returned for the authenticated session.

### Authentication flow

- JWT validation is configured in `Program.cs`.
- The token contains role and patient claims.
- The authentication middleware validates bearer tokens on protected endpoints.

### RBAC matrix

| Role | Endpoint/Resource | Access |
|------|-------------------|--------|
| Admin | `GET /api/patients` | Allowed |
| Admin | `GET /api/patients/{id}` | Allowed |
| Patient | `GET /api/patients/{id}` | Allowed only if same patient |
| Patient | `GET /api/patients` | Denied |
| Anonymous | Any protected endpoint | Denied |

### Authorization behavior

- Admins can browse and manage patient records.
- Patients can access only their own patient data and nested resources.
- Requests without a valid JWT are rejected.

### Pull Request

No verifiable PR link or mentor review information was available in the current repository state.

### Mentor review

Not verified in the current environment.

## 7. Final Status

- Authentication: Verified
- Registration: Verified
- Login: Verified
- RBAC: Verified
- Authorization: Verified
- Rejection testing: Verified by automated integration tests
- Sprint 2 backlog: No explicit backlog artifact found; follow-up item documented
- Sprint 3 backlog: Documented
- Retrospective: Completed
- Pull Request: Not verifiable from current environment
- Mentor review: Not verifiable from current environment
