# Hands-On Lab: Build the Patient Catalog Endpoint

## Goal

Build a production-style catalog endpoint for the primary resource in this project: `Patient`.

The endpoint must support:

- Pagination
- At least two optional filters
- Multiple sort options
- A response DTO instead of returning EF Core entities
- Postman verification with different query combinations

## Prerequisites

From the `CardiacPatientMonitoringDay3` directory, run:

```powershell
dotnet restore
dotnet test
```

The API requires authentication. Use the existing `Register` and `Login` requests in `CardiacPatientMonitoring.postman_collection.json` to obtain a JWT token.

## 1. Define the response and query DTOs

Add a query DTO for the catalog options and a generic paged response DTO in `CardiacPatientMonitoring.Api/DTOs/Dtos.cs`:

```csharp
public record PatientCatalogQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? Gender = null,
    DateOnly? BornBefore = null,
    string Sort = "nameAsc");

public record PagedResponseDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
```

`PatientResponseDto` is the item DTO. It exposes only the fields needed by the API client and prevents database entities from being returned directly.

## 2. Update the service contract

Change the patient service list method to accept the query object and return the paged DTO:

```csharp
Task<PagedResponseDto<PatientResponseDto>> GetAllAsync(
    PatientCatalogQuery query);
```

Keep the existing single-patient and CRUD methods unchanged.

## 3. Build the filtered query

In `PatientService`, start with a no-tracking `IQueryable`:

```csharp
var patients = db.Patients
    .AsNoTracking()
    .Where(p =>
        string.IsNullOrWhiteSpace(query.Search) ||
        p.FirstName.Contains(query.Search) ||
        p.LastName.Contains(query.Search));
```

Add the optional filters only when supplied:

```csharp
if (!string.IsNullOrWhiteSpace(query.Gender))
    patients = patients.Where(p => p.Gender == query.Gender);

if (query.BornBefore.HasValue)
    patients = patients.Where(p =>
        p.DateOfBirth < query.BornBefore.Value);
```

The endpoint therefore supports three filters: name search, exact gender, and date of birth before a supplied date.

## 4. Add sorting

Support at least two sort values. This implementation supports four:

```csharp
patients = query.Sort.ToLowerInvariant() switch
{
    "nameasc" => patients
        .OrderBy(p => p.LastName)
        .ThenBy(p => p.FirstName)
        .ThenBy(p => p.Id),
    "namedesc" => patients
        .OrderByDescending(p => p.LastName)
        .ThenByDescending(p => p.FirstName)
        .ThenBy(p => p.Id),
    "oldest" => patients
        .OrderBy(p => p.DateOfBirth)
        .ThenBy(p => p.Id),
    "newest" => patients
        .OrderByDescending(p => p.DateOfBirth)
        .ThenBy(p => p.Id),
    _ => throw new ArgumentException(
        "Sort must be nameAsc, nameDesc, oldest, or newest.")
};
```

The `Id` tie-breaker makes pagination stable when two patients have the same name or date of birth.

## 5. Apply pagination and project to the DTO

Validate the page values before querying:

```csharp
if (query.Page < 1 || query.PageSize is < 1 or > 100)
    throw new ArgumentException(
        "Page must be at least 1 and page size must be between 1 and 100.");
```

Count the filtered results before applying `Skip` and `Take`, then project directly to `PatientResponseDto`:

```csharp
var totalCount = await patients.CountAsync();
var totalPages = (int)Math.Ceiling(
    totalCount / (double)query.PageSize);

var items = await patients
    .Skip((query.Page - 1) * query.PageSize)
    .Take(query.PageSize)
    .Select(p => new PatientResponseDto(
        p.Id,
        p.FirstName,
        p.LastName,
        p.DateOfBirth,
        p.Gender,
        p.PhoneNumber))
    .ToListAsync();

return new(items, query.Page, query.PageSize, totalCount, totalPages);
```

This uses database-side filtering, sorting, counting, pagination, and projection.

## 6. Expose the endpoint from the controller

Update `PatientsController`:

```csharp
[HttpGet]
[ProducesResponseType<PagedResponseDto<PatientResponseDto>>(200)]
[ProducesResponseType(400)]
public Task<PagedResponseDto<PatientResponseDto>> GetAll(
    [FromQuery] PatientCatalogQuery query)
{
    return s.GetAllAsync(query);
}
```

The final endpoint is:

```text
GET /api/patients
```

Because the controller has `[Authorize]`, include this header:

```text
Authorization: Bearer <token>
```

## 7. Test with Postman

1. Start the API from the Day3 directory:

```powershell
dotnet run --project CardiacPatientMonitoring.Api
```

2. Open `CardiacPatientMonitoring.postman_collection.json`.
3. Run `Register` once, then run `Login`.
4. Confirm that the Login test saves the JWT in the collection `token` variable.
5. Send the following requests with the Authorization header.

### Pagination only

```text
GET {{baseUrl}}/api/patients?page=1&pageSize=2
```

Expected result: HTTP `200` with no more than two items.

### Gender filter and descending name sort

```text
GET {{baseUrl}}/api/patients?page=1&pageSize=5&gender=Female&sort=nameDesc
```

Expected result: only female patients, ordered by last name descending.

### Search, date filter, and oldest-first sort

```text
GET {{baseUrl}}/api/patients?page=2&pageSize=2&search=an&bornBefore=1995-01-01&sort=oldest
```

Expected result: matching patients on page two, ordered from oldest to newest.

### Invalid pagination

```text
GET {{baseUrl}}/api/patients?page=0&pageSize=101
```

Expected result: HTTP `400` with an error message.

## 8. Verify the response shape

A successful response has this structure:

```json
{
  "items": [
    {
      "id": 1,
      "firstName": "Alex",
      "lastName": "Morgan",
      "dateOfBirth": "1985-04-12",
      "gender": "Female",
      "phoneNumber": null
    }
  ],
  "page": 1,
  "pageSize": 2,
  "totalCount": 7,
  "totalPages": 4
}
```

## 9. Automated verification

Run the complete test suite:

```powershell
dotnet test
```

The tests should verify:

- The controller forwards catalog options to the service.
- An authenticated request returns the paged response.
- The response contains the seeded patient DTO.
- An unauthenticated request returns HTTP `401`.

## Completion checklist

- [x] Paginated `GET /api/patients` endpoint implemented
- [x] Optional `search`, `gender`, and `bornBefore` filters implemented
- [x] `nameAsc`, `nameDesc`, `oldest`, and `newest` sorting implemented
- [x] Paged response DTO implemented
- [x] Query projected directly to response DTO
- [x] Postman request combinations documented
- [x] Automated tests passing
