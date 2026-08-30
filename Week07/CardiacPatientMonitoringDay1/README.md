# Cardiac Patient Monitoring API

Day 1 of Sprint 2 adds ASP.NET Core Identity role infrastructure to the existing Cardiac Patient Monitoring API. The Sprint 1 patient, vital-sign, medication, appointment, JWT, and exception-handling functionality is preserved.

## Technology

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 with SQL Server
- ASP.NET Core Identity and JWT bearer authentication
- xUnit, Moq, and ASP.NET Core integration testing

## Identity and roles

`ApplicationDbContext` inherits from `IdentityDbContext<ApplicationUser>`. Identity is registered with Entity Framework stores and role support. The database contains the standard ASP.NET Identity tables plus the seeded roles:

- `Patient` — will be limited to their own monitoring information once patient-to-user ownership is introduced.
- `Admin` — will manage patient records and monitoring data across the system.

Day 1 does not add new endpoint-level role attributes. Existing protected controllers continue to require an authenticated user; the role and ownership rules are documented in [HandsOnLab.md](HandsOnLab.md) for later Sprint 2 implementation.

## Migrations

The existing `20260814130519_InitialCreate` migration already creates the ASP.NET Identity and Sprint 1 application tables. The Day 1 migration, `20260830100859_SeedIdentityRoles`, only inserts the `Admin` and `Patient` rows into `AspNetRoles`; it does not alter or remove any existing data or tables.

From this directory, apply migrations with:

```powershell
dotnet ef database update --project .\CardiacPatientMonitoring.Api --startup-project .\CardiacPatientMonitoring.Api
```

## Run the API

1. Ensure SQL Server Express is available and the `DefaultConnection` in `CardiacPatientMonitoring.Api/appsettings.json` is appropriate for your machine.
2. Apply the migrations.
3. Run:

```powershell
dotnet run --project .\CardiacPatientMonitoring.Api
```

The development Swagger UI is available at `/swagger`.
