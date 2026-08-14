# Cardiac Patient Monitoring System API

Standalone ASP.NET Core REST API for a cardiac monitoring training prototype. All included records are synthetic and must not be treated as clinical data.

## Stack and structure

C# / ASP.NET Core 10, EF Core with SQL Server, ASP.NET Core Identity + JWT, OpenAPI, xUnit, and Moq. The API project uses `Controllers`, `Models`, `DTOs`, `Data`, `Services`, and `Middleware`; tests are in `CardiacPatientMonitoring.Tests`.

## Configure and run

1. The default connection targets a local SQL Server Express instance (`.\\SQLEXPRESS`). Update `ConnectionStrings:DefaultConnection` in `CardiacPatientMonitoring.Api/appsettings.json` if yours uses another instance.
2. Replace the development `Jwt:Key` with a secret via user secrets before non-local use.
3. Run `dotnet ef database update --project CardiacPatientMonitoring.Api`.
4. Run `dotnet run --project CardiacPatientMonitoring.Api` and open the development OpenAPI endpoint shown by the app.

The migration seeds one synthetic patient with a vital sign, medication, and appointment. Use `dotnet test` to run tests.

## Authentication and endpoints

Register at `POST /api/auth/register`, then log in at `POST /api/auth/login`. Send the returned JWT as `Authorization: Bearer <token>` for protected routes.

- `GET/POST /api/patients`, `GET/PUT/DELETE /api/patients/{id}` (optional `search`)
- `GET/POST /api/patients/{patientId}/vital-signs`; individual vital signs use `/api/vital-signs/{id}`
- `GET/POST /api/patients/{patientId}/medications` (optional `search`); individual medication routes use `/api/medications/{id}`
- `GET/POST /api/patients/{patientId}/appointments` (optional `status`); individual appointment routes use `/api/appointments/{id}`

DTO data annotations return validation 400 responses. A centralized middleware returns safe JSON errors for missing resources and unexpected failures. Postman can use the same routes and bearer token flow.

## Local secrets and Swagger

On a new machine set the JWT key with `dotnet user-secrets set "Jwt:Key" "<long-random-secret>" --project CardiacPatientMonitoring.Api`. Do not commit it. Start the API, open `/swagger`, click **Authorize**, and enter `Bearer <token>` returned by login. Import the included Postman collection and set its base URL to the launch profile URL.
