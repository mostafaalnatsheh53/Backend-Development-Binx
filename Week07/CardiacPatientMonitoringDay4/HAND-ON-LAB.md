# Hands-On Lab — Implement Middleware & Open a Pull Request

## Objective

Implement one genuine cross-cutting concern as ASP.NET Core middleware, verify it across existing endpoints, and prepare the Day 4 work for review.

## 1. Cross-Cutting Concern

The selected concern is request correlation. A correlation ID lets a client and the API logs refer to the same request, including requests that fail before an endpoint returns normally. It applies to authentication, patient, and exception endpoints, and does not require changes to individual controllers.

ASP.NET Core supplies a trace identifier internally, but this application did not expose a stable request identifier to API clients. Custom middleware is appropriate because it can establish the identifier once at the request boundary and add it to every response centrally.

## 2. Middleware Implementation

`CorrelationIdMiddleware` is located at `CardiacPatientMonitoring.Api/Middleware/CorrelationIdMiddleware.cs`. It accepts a safe incoming `X-Correlation-ID` value, or generates a new identifier when one is absent, too long, or contains line breaks. It assigns the value to `HttpContext.TraceIdentifier` and the response header, then invokes the remainder of the pipeline.

It is registered in `Program.cs` immediately after `ExceptionHandlingMiddleware`, before HTTPS redirection, authentication, authorization, and controller endpoints. The response header is established before the next delegate runs, so it is available on successful responses, authorization failures, and exceptions handled by the outer exception middleware.

## 3. Endpoint Verification

| Endpoint | HTTP method | Expected behavior | Actual result | Verification method |
| --- | --- | --- | --- | --- |
| `/api/auth/register` | POST | Invalid date is rejected and a correlation ID is returned. | Verified: HTTP 400 with a non-empty `X-Correlation-ID` response header. | Automated integration test. |
| `/api/patients` | GET | Missing JWT is rejected and a correlation ID is returned. | Verified: HTTP 401 with a non-empty `X-Correlation-ID` response header. | Automated integration test. |
| `/api/test/exception` | GET | The controlled exception is converted to HTTP 500 and the requested correlation ID is preserved. | Verified: HTTP 500 with `X-Correlation-ID: test-correlation-id`. | Automated integration test. |

Manual/Postman verification was not performed in the current environment. The Postman collection is present, but no manual run was available to verify.

## 4. Authentication & RBAC

Authentication uses ASP.NET Core Identity for users and roles, with JWT bearer tokens issued by `AuthService`. Tokens include the user identifier, email, `patient_id`, and role claims. JWT validation uses the configured issuer, audience, signing key, lifetime, and role claim type.

The application defines `Admin` and `Patient` roles and the `AdminOnly` and `PatientOnly` policies. Controllers use `[Authorize]`, `[Authorize(Roles = "Admin")]`, and ownership checks for patient resources. Missing or invalid credentials produce unauthorized responses; authenticated users without the required role or ownership produce forbidden responses.

The effective request flow is exception handling, correlation ID middleware, HTTPS redirection, authentication, authorization, and mapped controller endpoints. Correlation is independent of identity and therefore also covers authentication and authorization failures.

## 5. Pull Request

- **Sprint 2 branch:** `feature/week07-day3-rbac-ownership` (the active branch containing this Day 4 project).
- **Commit:** To be recorded after the final test run.
- **Pull Request title:** `feat(week07): complete Day 4 middleware lab`
- **Pull Request summary:** Add correlation-ID middleware, verify it across anonymous, protected, and exception endpoints, and document the existing JWT/RBAC pipeline and verification.
- **Pull Request link:** Creation URL provided by GitHub: https://github.com/mostafaalnatsheh53/Backend-Development-Binx/pull/new/feature/week07-day3-rbac-ownership. The Pull Request itself is not created.
- **Reviewer:** Not verified in the current environment.
- **Feedback addressed:** No mentor feedback was found in the Day 4 project or available Git history.

## 6. Final Status

| Area | Status |
| --- | --- |
| Middleware implemented | Complete: `CorrelationIdMiddleware` added. |
| Middleware registered | Complete: registered in `Program.cs` after exception handling and before authentication. |
| Multiple endpoints verified | Complete: auth, patients, and exception endpoints verified by integration tests. |
| Authentication verified | Complete by existing authentication integration tests. |
| RBAC verified | Complete by existing role and ownership integration tests. |
| Tests executed | Complete: final full run passed 36/36 tests; focused middleware run passed 3/3. |
| Branch status | Active branch is `feature/week07-day3-rbac-ownership`. |
| Pull Request status | Not created; GitHub creation URL is available above. |
| Mentor review status | Not verified in the current environment. |
| Remaining manual steps | Push the commit and create the Pull Request, then add the actual link and reviewer when available. |