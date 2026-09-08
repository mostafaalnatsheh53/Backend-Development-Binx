# Hands-On Lab - Cache the Catalog Endpoint

## Objective

This lab adds distributed cache-aside behavior to the closest existing catalog-like endpoint in the Day 3 project. The project contains patients, medications, vital signs, and appointments, but it does not contain a `Product` entity, a catalog controller, or product create/update/delete endpoints. The actual global list surface is the admin-only patient list, so that endpoint is the one cached without inventing unrelated APIs.

## 1. Redis Setup

- Redis configuration is in `CardiacPatientMonitoring.Api/appsettings.json` under `ConnectionStrings:Redis`.
- Local Docker configuration is in `docker-compose.yml` and exposes Redis on `localhost:6379`.
- The API uses `Microsoft.Extensions.Caching.StackExchangeRedis` version `10.0.10`.
- Non-testing environments register Redis through `AddStackExchangeRedisCache` and use the configured connection string.
- The `Testing` environment uses `AddDistributedMemoryCache`, so automated tests do not require a running Redis server.
- The configured local connection uses no credentials or application secrets.
- Docker Compose configuration validation succeeded.
- Redis connectivity was not verified: Docker could not pull `redis:7-alpine` because the Docker Hub token request timed out.

## 2. Cache-Aside Implementation

- Endpoint: `GET /api/patients` for the unfiltered admin catalog-like list.
- Cache key: `catalog:patients:list:v1`.
- Cached format: JSON serialized `List<PatientResponseDto>`.
- Expiration: five minutes absolute, configured by `CacheSettings:CatalogExpirationMinutes`.
- Cache hit: deserialize the cached JSON and return it without querying EF Core.
- Cache miss: query patients with `AsNoTracking`, serialize the response, store it, and return it.
- Requests with a `search` query are intentionally not cached because their keys would require reliable variant tracking for invalidation. They query the database directly.

## 3. Cache Invalidation

Invalidation occurs in `PatientService` after the database save succeeds:

- Patient creation removes `catalog:patients:list:v1`.
- Patient update removes `catalog:patients:list:v1`.
- Patient deletion removes `catalog:patients:list:v1`.

The next unfiltered catalog request therefore misses the cache and reads fresh patient data. No product endpoints exist in this project, so product invalidation is not applicable to the actual codebase.

## 4. Cache Verification

The focused automated tests verified the cache behavior using the testing distributed-memory implementation:

- The first unfiltered list request populated the cache.
- A database change made directly after that request was not visible while the cached value remained.
- A patient update through `PatientService` removed the cache entry.
- The following list request returned the updated patient immediately.

The test result was 2 passed and 0 failed. Live Redis-backed API verification was not verified in the current environment because Docker could not pull the Redis image.

## 5. Performance Measurement

Endpoint-level cache miss and cache hit timings were not measured in the current environment. The API could not be exercised against the configured SQL Server and Redis stack after the Redis image pull failed. No timing values are fabricated.

| Request Type | Response Time | Database Query | Result |
|--------------|---------------|----------------|--------|
| Cache MISS | Not verified in the current environment. | Expected: Yes; live endpoint not run | Not verified in the current environment. |
| Cache HIT | Not verified in the current environment. | Expected: No; live endpoint not run | Not verified in the current environment. |

## 6. Before / After Results

| Scenario | Response Time | DB Query | Cache |
|----------|---------------|----------|-------|
| Cache MISS | Not verified in the current environment. | Not verified in the current environment. | MISS behavior implemented |
| Cache HIT | Not verified in the current environment. | Not verified in the current environment. | HIT behavior implemented |
| After Patient Update | Not verified in the current environment. | Not verified in the current environment. | Cache removed; fresh read behavior tested |

## 7. Testing

- API build: passed.
- Cache-focused automated tests: passed, 2/2.
- Full automated test suite: passed, 38/38.
- Docker Compose configuration: passed validation.
- Redis PING: not verified because the image could not be pulled.
- Live API cache hit/miss and product create/update/delete flows: not verified in the current environment.

## 8. Final Status

- Redis setup: configuration and Compose file completed; container startup not verified.
- `IDistributedCache`: registered as Redis outside Testing and memory cache in Testing.
- Cache-aside: implemented for the actual unfiltered patient list.
- Expiration: five minutes, configuration-driven.
- Cache invalidation: implemented for patient create, update, and delete.
- Product invalidation: not applicable; no product endpoints exist in this project.
- Cache testing: focused behavior tests passed.
- Performance measurement: not verified in the current environment.
- Documentation: completed.
