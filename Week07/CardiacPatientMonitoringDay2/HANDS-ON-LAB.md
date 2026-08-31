# Hands-On Lab — Registration and Login

## Objective

Complete a registration and login flow that links each authenticated `ApplicationUser` to the project's domain `Patient` record.

## Starting Project State

The API already used ASP.NET Core Identity through `ApplicationUser : IdentityUser`, `ApplicationDbContext : IdentityDbContext<ApplicationUser>`, `UserManager<ApplicationUser>`, and bearer JWT authentication configured from the `Jwt` configuration section. `AuthController` already exposed `POST /api/auth/register` and `POST /api/auth/login`, but registration only created an Identity user and login issued a token without resolving a `Patient`.

## 1. Patient → IdentityUser Relationship

### Existing Model

`Patient` is the project's domain entity corresponding to the lab's Customer. `ApplicationUser` is the Identity implementation.

### Changes Implemented

`Patient.UserId` is a required string FK to `ApplicationUser.Id`; `Patient.User` and `ApplicationUser.Patient` are navigation properties. This is a required one-to-one relationship from Patient to ApplicationUser, with an optional Patient navigation from ApplicationUser so that an Identity user can exist before its profile is created inside the registration transaction.

### EF Core Configuration

`ApplicationDbContext.OnModelCreating` configures `Patient.UserId` with `HasOne(p => p.User).WithOne(u => u.Patient).HasForeignKey<Patient>(p => p.UserId).IsRequired()`.

### Delete Behavior

The FK uses `DeleteBehavior.Cascade`: a Patient is an owned domain profile for an Identity user, so deleting the user removes its linked Patient and the Patient's existing dependent records follow their established cascade relationships.

### Database Migration

`20260831190411_LinkPatientToApplicationUser` adds non-null `Patients.UserId`, a unique index, and `FK_Patients_AspNetUsers_UserId`. It also seeds the existing sample Patient with an Identity user so the required FK is valid. The migration was generated and inspected. Applying it was attempted but not completed because the configured SQL Server instance reported that encryption is required but unsupported by this machine.

## 2. Registration Endpoint

### Endpoint

`POST /api/auth/register` in `AuthController.Register` delegates to `IAuthService.RegisterAsync` and returns `201 Created` with `AuthResponseDto`.

### Request and Validation

`RegisterDto` contains `Email`, `Password`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, and optional `PhoneNumber`. Data annotations validate email, password length, required names/gender, lengths, and phone format. `AuthService` additionally rejects missing or future dates of birth.

### Creation and Transaction

`AuthService.RegisterAsync` validates Patient details, creates `ApplicationUser` through `UserManager.CreateAsync` (Identity performs password hashing), then saves a `Patient` with the generated `ApplicationUser.Id`. For relational providers it opens an EF Core transaction before creating the user and commits only after the Patient save succeeds; failures dispose the transaction and roll back both writes. The in-memory test provider has no relational transactions, so the service deliberately skips transaction creation only in that test-provider case.

### Response and Errors

On success the response contains the token, configured expiration time, and generated `PatientId`; it contains neither password nor password hash. Invalid request/model validation and Identity failures return `400` through the existing problem-details convention. Invalid login credentials or an unlinked user return `401` without revealing which condition failed.

## 3. Login Endpoint

### Endpoint and Authentication Flow

`POST /api/auth/login` receives `LoginDto`. `AuthService.LoginAsync` finds the Identity user by email, verifies the password with `UserManager.CheckPasswordAsync`, then resolves the Patient using `Patient.UserId`.

### JWT Generation

The token uses `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`, and `Jwt:ExpirationMinutes` from configuration. Its standard claims are `ClaimTypes.NameIdentifier` (Identity user ID) and `ClaimTypes.Email`; its domain mapping claim is `patient_id`, with the linked integer `Patient.Id` as its value. No signing key or token is documented here.

## 4. Postman Testing

The existing `CardiacPatientMonitoring.postman_collection.json` was updated so Register supplies the required Patient fields. Postman itself was not executed in this environment.

Equivalent automated API verification ran in `AuthEndpointIntegrationTests.RegisterThenLogin_CreatesLinkedPatientAndJwtClaim`: it registered a unique user, checked `AspNetUsers`/`Patients` through `ApplicationDbContext`, logged in, decoded the returned JWT, and verified `patient_id`, issuer, audience, and expiration. The test uses an EF Core in-memory test database, not the configured SQL Server database.

## 5. Git / Feature Branch

The implementation branch is `feature/week07-day2-registration-login`. The implementation commit is `0afe19c229579b21e091f52ac054a4402141ea2f` (`feat(auth): implement registration and login flow`). This documentation is committed separately after recording that verified hash.

## 6. Validation Results

- `dotnet build CardiacPatientMonitoring.slnx --no-restore`: passed (before the final test addition).
- `dotnet test CardiacPatientMonitoring.slnx --no-restore`: passed, 31/31 tests.
- Migration scaffold: passed; migration source and snapshot were generated.
- SQL Server migration apply: not applied; local SQL Server encryption support prevented connection.
- Registration/login/database/JWT validation: passed through the in-memory API integration test.

## 7. Problems Encountered

The configured local SQL Server could not establish an encrypted connection (`Error Number: 20`), so direct database migration and Postman-on-SQL-Server verification remain manual steps. NuGet vulnerability metadata retrieval also emitted `NU1900` warnings because the NuGet service index was unreachable; restore artifacts were already available and build/tests succeeded.

## 8. Security Notes

Passwords go only to `UserManager.CreateAsync` and `CheckPasswordAsync`; the application never hashes or returns them. JWT signing inputs are read from configuration, and login returns a generic unauthorized result for both bad passwords and absent Patient links. The existing development JWT key remains in `appsettings.json`; moving it to User Secrets or deployment secret storage is a future configuration hardening task.

## 9. Final Result

Registration atomically creates a password-hashed Identity user and linked Patient for relational databases. Login authenticates that user, resolves the Patient, and returns a configured JWT containing `patient_id` for downstream domain identification.
