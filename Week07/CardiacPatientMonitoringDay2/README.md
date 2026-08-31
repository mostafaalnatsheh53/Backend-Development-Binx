# Week 07 — Day 2

## Registration & Login Implementation

### Day Objective

Connect the Cardiac Patient Monitoring API's Identity users to its Patient domain model, then implement complete, testable registration and login flows.

### Project

`CardiacPatientMonitoringDay2` is an ASP.NET Core API for patients, vital signs, medications, and appointments. It uses EF Core, SQL Server in normal environments, ASP.NET Core Identity, and JWT bearer authentication.

### Starting Point

`ApplicationUser` already extended `IdentityUser`, and `AuthController` already exposed register/login routes. Those routes created/authenticated Identity users and generated a JWT, but `Patient` had no Identity relationship and the token could not identify a Patient.

### Day Plan

- Add the Patient-to-ApplicationUser one-to-one FK.
- Extend registration input with required Patient profile data.
- Create user and Patient together in a relational transaction.
- Resolve the Patient during login and add its ID to the JWT.
- Update the Postman collection, migration, tests, and documentation.

### 1. IdentityUser and Domain Entity Relationship

The domain Customer named in the lab is `Patient`; the Identity type is `ApplicationUser`. `Patient.UserId` is a required FK to `ApplicationUser.Id`, `Patient.User` navigates to the user, and `ApplicationUser.Patient` navigates back. EF Core configures a unique required FK and cascade deletion from user to Patient.

### 2. Registration Implementation

`POST /api/auth/register` accepts `RegisterDto`, validates profile details, creates `ApplicationUser` through Identity, then creates `Patient` using the resulting user ID. Relational execution is one transaction.

```text
Client → POST /api/auth/register → UserManager.CreateAsync
       → Patient(UserId) → transaction commit → 201 AuthResponseDto
```

### 3. Login Implementation

`POST /api/auth/login` accepts `LoginDto`, finds the user by email, checks the password through Identity, finds the Patient with the same `UserId`, and returns a token. Missing users, bad passwords, and users without a Patient link receive the same generic `401` response.

```text
Client → POST /api/auth/login → Identity password check
       → Patient lookup → JWT with patient_id → 200 AuthResponseDto
```

### 4. JWT Authentication

`AuthService` reads issuer, audience, signing key, and `ExpirationMinutes` from the `Jwt` configuration section. Tokens include the Identity name identifier, email, and `patient_id`; the latter is the linked `Patient.Id`. Secrets and token values are intentionally not documented.

### 5. Database Changes

Migration `20260831190411_LinkPatientToApplicationUser` adds `Patients.UserId`, its unique index, and its FK to `AspNetUsers`. The existing seeded Patient is linked to a seeded Identity user. The migration was scaffolded and reviewed but could not be applied to `.\SQLEXPRESS` because its required encryption is unsupported by this machine.

### 6. API Endpoints

| Method | Route | Purpose | Authentication |
| ------ | ----- | ------- | -------------- |
| POST | `/api/auth/register` | Create linked Identity user and Patient | Anonymous |
| POST | `/api/auth/login` | Authenticate and receive JWT with `patient_id` | Anonymous |

### 7. DTOs and Models

`RegisterDto` now includes identity credentials plus Patient profile fields. `LoginDto` contains email and password. `AuthResponseDto` returns `Token`, `ExpiresAt`, and `PatientId`. `Patient` now has `UserId` and `User`; `ApplicationUser` has `Patient`.

### 8. Postman Testing

The existing collection's Register request now includes the Patient profile fields. Postman was not available/executed. Equivalent API coverage passed in the in-memory integration test: registration verified the linked `ApplicationUser` and `Patient`; login yielded a nonempty JWT; decoded claims confirmed `patient_id`, issuer, audience, and expiration.

### 9. Git

Branch: `feature/week07-day2-registration-login`. Implementation commit: `0afe19c229579b21e091f52ac054a4402141ea2f` — `feat(auth): implement registration and login flow`. Documentation is committed separately after recording that verified implementation hash.

### 10. Validation

`dotnet test CardiacPatientMonitoring.slnx --no-restore` passed all 31 tests. The migration scaffold and code build passed. Direct SQL Server migration/database validation was blocked by the local encryption error, while equivalent in-memory API/database/JWT validation passed.

### 11. Challenges and Solutions

SQL Server could not connect because it requires encryption unavailable on this machine. The schema migration is retained for application in a compatible development environment. A `NU1900` package-vulnerability metadata warning was also observed due to unreachable NuGet metadata, without preventing build/tests.

### 12. Security Considerations

Identity owns password hashing and verification. Registration is transactional for relational databases, login errors are generic, JWT settings are configuration-driven, and no password, token, or signing key is written to this documentation. The existing plaintext development key should be moved to User Secrets or deployment secret storage in future work.

### 13. Final Day Outcome

The API now creates and authenticates linked Identity users and Patients, and carries the authenticated Patient's ID as `patient_id` in each JWT.

### 14. Files Changed

- `CardiacPatientMonitoring.Api/Models/Entities.cs`
- `CardiacPatientMonitoring.Api/Data/ApplicationDbContext.cs`
- `CardiacPatientMonitoring.Api/DTOs/Dtos.cs`
- `CardiacPatientMonitoring.Api/Services/Services.cs`
- `CardiacPatientMonitoring.Api/appsettings.json`
- `CardiacPatientMonitoring.Api/Migrations/20260831173949_LinkPatientToApplicationUser.cs` and designer/snapshot
- `CardiacPatientMonitoring.Tests/AuthServiceTests.cs`
- `CardiacPatientMonitoring.Tests/AuthEndpointIntegrationTests.cs`
- `CardiacPatientMonitoring.postman_collection.json`
- `HANDS-ON-LAB.md`
- `README.md`
