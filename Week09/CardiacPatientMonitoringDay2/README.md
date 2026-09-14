# Cardiac Patient Monitoring API

## Overview

This ASP.NET Core API supports a cardiac patient monitoring workflow. It provides JWT authentication, administrator-managed patient records, patient-scoped vital signs, medications, and appointments, plus middleware diagnostics for controlled exception handling. Patient-owned records are protected by authorization and patient identity claims.

## Tech Stack

- .NET 10 and ASP.NET Core Web API
- Entity Framework Core 10 with SQL Server
- EF Core InMemory provider for integration tests
- ASP.NET Core Identity and JWT bearer authentication
- Redis distributed cache
- Swashbuckle Swagger/OpenAPI
- xUnit, Moq, and ASP.NET Core WebApplicationFactory
- Postman collection for endpoint checks

## Project Structure

- `CardiacPatientMonitoring.Api/` - API project, controllers, services, DTOs, data access, middleware, and EF Core migrations.
- `CardiacPatientMonitoring.Tests/` - unit and integration tests.
- `CardiacPatientMonitoring.postman_collection.json` - Postman requests grouped by middleware, authentication, patients, vital signs, medications, and appointments.
- `ERD/ERD.png` - database relationship diagram.
- `docker-compose.yml` - optional Redis container for local development.
- `Hands-On-Lab.md` - documentation completion record for Week 09 Day 2.

## Prerequisites

- .NET SDK 10.0 or later compatible with `net10.0`.
- SQL Server or SQL Server Express reachable using the configured `DefaultConnection`.
- Redis on `localhost:6379`, or an equivalent configured Redis endpoint, when running outside the Testing environment. Docker Desktop can start the included Redis service.
- Optional: Postman for collection execution.

## Setup

1. Open a terminal in `Week09/CardiacPatientMonitoringDay2`.
2. Configure the values described in [Configuration / Environment Variables](#configuration--environment-variables). Do not commit real credentials or signing keys.
3. Start Redis if needed:

   ```powershell
   docker compose up -d redis
   ```

4. Restore dependencies:

   ```powershell
   dotnet restore CardiacPatientMonitoring.slnx
   ```

5. Apply the existing EF Core migrations:

   ```powershell
   dotnet ef database update --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
   ```

## Configuration / Environment Variables

ASP.NET Core configuration can be supplied through `appsettings.json`, user secrets, or environment variables using the `__` separator. The checked-in development settings do not add alternate connection values.

| Configuration key | Purpose | Example placeholder |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server database connection | `Server=.\\SQLEXPRESS;Database=CardiacPatientMonitoringDb;Trusted_Connection=True;TrustServerCertificate=True` |
| `ConnectionStrings:Redis` | Redis cache connection | `localhost:6379,abortConnect=false` |
| `Jwt:Issuer` | JWT issuer validation value | `CardiacPatientMonitoring` |
| `Jwt:Audience` | JWT audience validation value | `CardiacPatientMonitoringClient` |
| `Jwt:Key` | JWT signing key; keep secret and use at least 32 bytes | `YOUR_SECRET_HERE` |
| `Jwt:ExpirationMinutes` | Token lifetime setting | `120` |
| `InitialAdmin:Email` | Optional development administrator email | `admin@example.com` |
| `InitialAdmin:Password` | Optional development administrator password | `YOUR_ADMIN_PASSWORD_HERE` |
| `ASPNETCORE_ENVIRONMENT` | Selects the ASP.NET Core environment | `Development` |

The values shown in the table are configuration names and safe examples. Replace secrets before running the API. `InitialAdmin` is only used when both values are non-empty.

## Database Setup

The API uses SQL Server through `ApplicationDbContext`. In Development, startup calls `Database.MigrateAsync()` and then runs the development data seeder. The migration files are under `CardiacPatientMonitoring.Api/Migrations`.

For a clean database, configure `ConnectionStrings:DefaultConnection` and run:

```powershell
dotnet ef database update --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
```

## Migrations

Existing migrations include the initial schema, Identity role seeding, patient-to-user linking, and performance indexes. To create a new migration after a model change:

```powershell
dotnet ef migrations add DescribeYourChange --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
```

Apply it with `dotnet ef database update` as shown above. Review generated migrations before applying them to shared databases.

## Running the API

From `Week09/CardiacPatientMonitoringDay2`:

```powershell
dotnet run --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj --launch-profile https
```

The configured Development URLs are [https://localhost:7213](https://localhost:7213) and [http://localhost:5222](http://localhost:5222). A local development certificate may need to be trusted for HTTPS.

## Swagger / API Documentation

Swagger UI is enabled in Development at [https://localhost:7213/swagger](https://localhost:7213/swagger). The generated OpenAPI JSON is available at [https://localhost:7213/swagger/v1/swagger.json](https://localhost:7213/swagger/v1/swagger.json) when the API is running. XML comments are generated by the API project and included by the existing Swashbuckle setup.

## Authentication

Authentication uses JWT bearer tokens. Register or log in through `POST /api/auth/register` or `POST /api/auth/login`, then send the returned token as:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

The application defines `Admin` and `Patient` roles. Patient records can be read by authorized users subject to the controller checks; patient creation, update, deletion, and the patient catalog require the Admin role. The patient-scoped clinical resources validate the current patient's access.

## API Endpoints

- `POST /api/auth/register`, `POST /api/auth/login`
- `GET|POST /api/patients`, `GET|PUT|DELETE /api/patients/{id}`
- `GET|POST /api/patients/{patientId}/vital-signs`, `GET|PUT|DELETE /api/vital-signs/{id}`
- `GET|POST /api/patients/{patientId}/medications`, `GET|PUT|DELETE /api/medications/{id}`
- `GET|POST /api/patients/{patientId}/appointments`, `GET|PUT|DELETE /api/appointments/{id}`
- `GET /api/test/exception` for controlled exception-middleware testing

## Testing

Run the full test project with:

```powershell
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj
```

The test host uses the InMemory EF Core provider and distributed memory cache under the `Testing` environment. The Postman collection is `CardiacPatientMonitoring.postman_collection.json`; import it into Postman, set `baseUrl` and account data as needed, run Login to populate `token`, and then run the grouped requests. The collection contains 23 requests and one test script for each request. Executing the collection against a running API is **Needs Manual Verification**.

## Documentation

- [Swagger UI](https://localhost:7213/swagger)
- [Postman collection](CardiacPatientMonitoring.postman_collection.json)
- [Hands-On Lab completion record](Hands-On-Lab.md)
- [Database ERD](ERD/ERD.png)

## Troubleshooting

- If SQL Server cannot be reached, verify the `DefaultConnection` server name and that SQL Server Express is running.
- If the API fails while configuring Redis, start `docker compose up -d redis` or provide a reachable `ConnectionStrings:Redis` value.
- If HTTPS is untrusted locally, run `dotnet dev-certs https --trust` where supported, or use the HTTP profile at `http://localhost:5222`.
- If protected calls return `401`, log in again and send the returned JWT in the Authorization header.
- If an administrative call returns `403`, use an account with the `Admin` role.
- If migrations fail, confirm the EF tool is installed and the configured SQL Server login can create/update `CardiacPatientMonitoringDb`.

## Verification Checklist

- [x] Project builds successfully.
- [ ] Database is configured. Needs Manual Verification.
- [ ] Migrations apply successfully. Needs Manual Verification.
- [ ] API starts successfully. Needs Manual Verification.
- [ ] Swagger loads successfully. Needs Manual Verification.
- [ ] Authentication works end to end. Needs Manual Verification.
- [ ] Tests pass. Needs Manual Verification.
- [x] Postman collection is complete by static route and script audit.
