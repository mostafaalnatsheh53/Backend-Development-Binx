# Week 08 - Day 3: Cache the Catalog Endpoint

## Overview

This day introduces Redis-backed distributed caching and the cache-aside pattern. The supplied Day 3 project is a cardiac patient monitoring API. It has no `Product` entity or catalog/product endpoints, so the implementation applies the requested catalog behavior to the actual global list surface: the admin-only `GET /api/patients` endpoint.

## Learning Objectives

- Redis and distributed caching.
- `IDistributedCache` registration.
- Cache-aside reads.
- Cache expiration.
- Cache invalidation after writes.
- Cache hit versus cache miss behavior.
- Performance measurement and honest verification.

## Redis Setup

`CardiacPatientMonitoring.Api` references `Microsoft.Extensions.Caching.StackExchangeRedis` 10.0.10. Non-testing environments register `AddStackExchangeRedisCache` using `ConnectionStrings:Redis` from configuration. The local value is `localhost:6379,abortConnect=false`; no credentials are stored in source.

`docker-compose.yml` defines a local `redis:7-alpine` service on port 6379. Docker Compose configuration validation passed, but the image pull timed out while contacting Docker Hub, so Redis connectivity was not verified. Automated Testing uses `AddDistributedMemoryCache` instead.

## Cache-Aside Pattern

1. Request the unfiltered patient catalog.
2. Check `IDistributedCache`.
3. On a cache hit, deserialize and return the JSON response.
4. On a cache miss, query the database.
5. Serialize the response and store it for five minutes.
6. Return the response.

Filtered patient searches bypass caching because they are query variants and must not be returned under the unfiltered key.

## Catalog Caching

- Endpoint: `GET /api/patients`.
- Cache key: `catalog:patients:list:v1`.
- Cached response: JSON `List<PatientResponseDto>`.
- Expiration: five-minute absolute expiration from `CacheSettings:CatalogExpirationMinutes`.
- Cache hit: no EF query is executed by `PatientService`.
- Cache miss: EF Core loads the list with a no-tracking projection, then the result is cached.

## Cache Invalidation

### Product Create

No product create endpoint or product service exists in this project. Product invalidation is therefore not applicable without inventing an API.

### Product Update

No product update endpoint or product service exists. The actual patient update path removes `catalog:patients:list:v1` after `SaveChangesAsync` succeeds.

### Product Delete

No product delete endpoint or product service exists. The actual patient delete path removes `catalog:patients:list:v1` after `SaveChangesAsync` succeeds.

Removing the key after each actual patient write prevents the next unfiltered catalog response from using stale data.

## Cache Verification

The focused tests verified an initial cache population, reuse of the cached result, and fresh data after a patient update invalidated the key. The result was 2 passed and 0 failed.

Live Redis/API verification, including product create/update/delete, was not verified in the current environment. The Redis image could not be downloaded by Docker, and the project does not contain product endpoints to execute.

## Performance Measurements

Endpoint timing was not measured because the live SQL Server/Redis API path was unavailable. No timing or database-query result is fabricated.

| Request Type | Response Time | DB Query | Cache |
|--------------|---------------|----------|-------|
| Cache MISS | Not verified in the current environment. | Not verified in the current environment. | MISS behavior implemented |
| Cache HIT | Not verified in the current environment. | Not verified in the current environment. | HIT behavior implemented |

## Testing & Verification

- API project build: passed.
- Cache-focused tests: passed, 2/2.
- Full test suite: passed, 38/38.
- Docker Compose configuration: passed validation.
- Redis connectivity: not verified because Docker Hub image resolution timed out.
- Live cache miss/hit timing: not verified in the current environment.
- Product invalidation: not applicable because the endpoints do not exist.

## Files Changed

- `CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj` - Redis cache package.
- `CardiacPatientMonitoring.Api/Program.cs` - distributed-cache registration.
- `CardiacPatientMonitoring.Api/appsettings.json` - Redis and expiration configuration.
- `CardiacPatientMonitoring.Api/Services/Services.cs` - patient catalog cache-aside and invalidation.
- `CardiacPatientMonitoring.Tests/PatientCatalogCacheTests.cs` - cache hit and invalidation tests.
- `docker-compose.yml` - local Redis service.
- `HAND-ON-LAB.md` - complete lab notes and verification status.
- `README.md` - complete Day 3 overview and results.

## What I Learned

Distributed cache entries need a stable key, an explicit serialization format, and an expiration policy. Cache invalidation after successful writes is required for consistency. Cache variants must include their query shape or bypass caching, and performance claims must come from actual endpoint measurements rather than assumptions.

## Final Status

| Area | Status |
|------|--------|
| Redis | Configured; container not verified because image pull timed out |
| IDistributedCache | Completed |
| Cache-Aside | Completed for the actual patient catalog-like list |
| Catalog Cache | Completed for unfiltered `GET /api/patients` |
| Cache Expiration | Completed; five minutes |
| Create Invalidation | Completed for patient create; product endpoint not applicable |
| Update Invalidation | Completed for patient update; product endpoint not applicable |
| Delete Invalidation | Completed for patient delete; product endpoint not applicable |
| Cache Testing | Passed 2/2 focused tests |
| Performance Measurement | Not verified in the current environment |
| Documentation | Completed |
