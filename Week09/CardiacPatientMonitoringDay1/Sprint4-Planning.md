# Sprint 4 Planning

## Sprint Goal

Close the highest-risk API test gaps and make the Sprint 3 performance verification repeatable. Sprint 4 will prioritize authentication and authorization behavior, patient ownership boundaries, and business-critical clinical endpoints while carrying forward the Sprint 3 retrospective action: add a repeatable performance verification suite that runs at least 50 patients, asserts query counts, and stores cache HIT/MISS evidence as build artifacts.

## Sprint 4 Backlog

| ID | Task | Description | Priority | Risk | Acceptance Criteria | Status |
|---|---|---|---|---|---|---|
| S4-01 | Carry forward Sprint 3 performance verification action | Build the repeatable query-count and cache-evidence suite selected in the Sprint 3 retrospective. | High | High | Exercise selected list endpoints with at least 50 patients; assert no more than one SQL command per request; capture cache HIT/MISS evidence as build artifacts. | NOT STARTED |
| S4-02 | Cover patient administration update/delete endpoints | Add endpoint-level happy and error tests for admin-only patient update and delete, including non-admin denial and missing-resource behavior. | High | High | Tests verify an Admin can update/delete, a Patient receives `403`, and a missing resource returns the API's actual not-found response. | NOT STARTED |
| S4-03 | Cover clinical resource ownership matrix | Add endpoint tests for vital-sign, medication, and appointment routes covering anonymous access, owner access, cross-patient denial, and missing resources where applicable. | High | High | Each clinical resource family has tests proving owner access, cross-patient `403`, anonymous `401`, and not-found behavior for item routes. | PARTIALLY COMPLETE |
| S4-04 | Cover clinical CRUD happy/error paths | Add tests for the untested list, create, update, and delete routes, using valid and invalid DTOs without changing production validation. | Medium | High | Every clinical route has at least one successful or realistic error-path endpoint test, with validation and missing-resource cases represented. | NOT STARTED |
| S4-05 | Cover cache invalidation endpoint flows | Extend endpoint-level coverage for patient create, update, and delete to verify the catalog is fresh after each write. | Medium | Medium | Each write flow is exercised through HTTP and the next unfiltered catalog read reflects the change. | NOT STARTED |

S4-01 is the exact action selected in the Sprint 3 retrospective. S4-03 is partially complete because this day's new tests cover cross-patient medication and appointment reads and invalid vital-sign creation; the full ownership matrix remains open.
