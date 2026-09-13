# Endpoint Test Coverage Audit

## Scope

This audit lists every controller action discovered in `CardiacPatientMonitoring.Api/Controllers`. There are 23 endpoints: 2 authentication endpoints, 5 patient endpoints, 5 vital-sign endpoints, 5 medication endpoints, 5 appointment endpoints, and 1 controlled exception endpoint. No minimal APIs or payment/payment-adjacent endpoints were found.

`HAPPY PATH` and `ERROR PATH` indicate whether the test project contains a test for that behavior. Direct controller tests are identified as such; they are not treated as live HTTP integration tests. `BOTH`, `HAPPY PATH`, `ERROR PATH`, and `NEITHER` are the resulting coverage classifications.

## Endpoint Matrix

| Method | Endpoint | Controller | Authentication | Role/Policy | Happy Path | Error Path | Status | Risk |
|---|---|---|---|---|---|---|---|---|
| POST | `/api/auth/register` | `AuthController.Register` | No | None | Yes, integration | Yes, invalid date integration | BOTH | High |
| POST | `/api/auth/login` | `AuthController.Login` | No | None | Yes, integration | Yes, invalid password integration added this day | BOTH | High |
| GET | `/api/patients` | `PatientsController.GetAll` | Yes | `Admin` | Yes, integration | Yes, anonymous `401` and Patient `403` integration | BOTH | High |
| GET | `/api/patients/{id}` | `PatientsController.Get` | Yes | Admin or matching patient | No | Yes, missing patient and cross-patient `403` | ERROR PATH | High |
| POST | `/api/patients` | `PatientsController.Create` | Yes | `Admin` | Yes, direct controller test | Yes, Patient `403` integration | BOTH | High |
| PUT | `/api/patients/{id}` | `PatientsController.Update` | Yes | `Admin` | No | No endpoint test | NEITHER | High |
| DELETE | `/api/patients/{id}` | `PatientsController.Delete` | Yes | `Admin` | Yes, direct controller test | No endpoint test | HAPPY PATH | High |
| GET | `/api/patients/{patientId}/vital-signs` | `VitalSignsController.All` | Yes | Admin or matching patient | No | No endpoint test | NEITHER | High |
| GET | `/api/vital-signs/{id}` | `VitalSignsController.Get` | Yes | Admin or owning patient | No | No endpoint test | NEITHER | High |
| POST | `/api/patients/{patientId}/vital-signs` | `VitalSignsController.Create` | Yes | Admin or matching patient | Yes, direct controller test | Yes, invalid input integration added this day | BOTH | High |
| PUT | `/api/vital-signs/{id}` | `VitalSignsController.Update` | Yes | Admin or owning patient | No | No endpoint test | NEITHER | High |
| DELETE | `/api/vital-signs/{id}` | `VitalSignsController.Delete` | Yes | Admin or owning patient | No | No endpoint test | NEITHER | High |
| GET | `/api/patients/{patientId}/medications` | `MedicationsController.All` | Yes | Admin or matching patient | No | No endpoint test | NEITHER | High |
| GET | `/api/medications/{id}` | `MedicationsController.Get` | Yes | Admin or owning patient | No | Yes, cross-patient `403` integration added this day | ERROR PATH | High |
| POST | `/api/patients/{patientId}/medications` | `MedicationsController.Create` | Yes | Admin or matching patient | No | No endpoint test | NEITHER | High |
| PUT | `/api/medications/{id}` | `MedicationsController.Update` | Yes | Admin or owning patient | No | No endpoint test | NEITHER | High |
| DELETE | `/api/medications/{id}` | `MedicationsController.Delete` | Yes | Admin or owning patient | No | Yes, missing resource direct controller test | ERROR PATH | High |
| GET | `/api/patients/{patientId}/appointments` | `AppointmentsController.All` | Yes | Admin or matching patient | No | No endpoint test | NEITHER | High |
| GET | `/api/appointments/{id}` | `AppointmentsController.Get` | Yes | Admin or owning patient | No | Yes, cross-patient `403` integration added this day | ERROR PATH | High |
| POST | `/api/patients/{patientId}/appointments` | `AppointmentsController.Create` | Yes | Admin or matching patient | Yes, direct controller test | No endpoint test | HAPPY PATH | High |
| PUT | `/api/appointments/{id}` | `AppointmentsController.Update` | Yes | Admin or owning patient | No | No endpoint test | NEITHER | High |
| DELETE | `/api/appointments/{id}` | `AppointmentsController.Delete` | Yes | Admin or owning patient | No | No endpoint test | NEITHER | High |
| GET | `/api/test/exception` | `TestController.TriggerException` | No | None | No, endpoint intentionally throws | Yes, `500` middleware integration | ERROR PATH | Medium |

## Test File Mapping

- `AuthEndpointIntegrationTests.cs`: registration/login flow, admin denial, ownership denial, and invalid-login test added this day.
- `PatientEndpointIntegrationTests.cs`: patient catalog happy path and anonymous denial.
- `HighRiskEndpointIntegrationTests.cs`: cross-patient medication and appointment denial plus invalid vital-sign creation added this day.
- `PatientsControllerTests.cs`: direct patient create happy path and patient not-found behavior.
- `ControllerAndMiddlewareTests.cs`: direct controller smoke tests for patient delete, vital create, medication delete error, appointment create, auth controller behavior, and middleware mappings.
- `CorrelationIdMiddlewareIntegrationTests.cs`: invalid registration response, protected patient denial, and controlled exception response.
- Service tests verify selected service behavior, but are not counted as endpoint coverage unless a controller or HTTP test also exists.

## Coverage Summary

| Classification | Count |
|---|---:|
| BOTH | 5 |
| HAPPY PATH | 2 |
| ERROR PATH | 3 |
| NEITHER | 13 |
| Total endpoints | 23 |

The 13 `NEITHER` endpoints are the main remaining endpoint-coverage gap. This is an endpoint test classification, not a statement that the underlying services have no tests.

# Test Gap Prioritization

| Priority | Endpoint | Risk | Missing Coverage | Reason |
|---|---|---|---|---|
| 1 | `PUT /api/patients/{id}` | High | Happy and error endpoint paths | Admin-only mutation can alter patient identity data; neither authorization denial nor missing-resource behavior is tested through HTTP. |
| 2 | `GET /api/vital-signs/{id}`, `PUT /api/vital-signs/{id}`, `DELETE /api/vital-signs/{id}` | High | Ownership, anonymous, not-found, and mutation paths | Clinical readings are sensitive patient data and the item routes enforce ownership in controller code. |
| 3 | Medication create/list/update/delete routes | High | Most happy and authorization paths | Medication records are clinical treatment data; only a missing-resource direct controller case and the new cross-patient read denial are covered. |
| 4 | Appointment list/create/update/delete routes | High | Most happy and authorization paths | Appointments are patient-specific operational data; only direct create and the new cross-patient read denial are covered. |
| 5 | Vital-sign list route | High | Happy, anonymous, ownership, and not-found paths | The service has validation tests, but no endpoint test proves the route authorization and response contract. |

Authentication was required to be ranked first by the lab instructions. The authentication endpoints were audited first and now have both happy and error endpoint coverage, so no authentication gap remains in the selected top gaps after this day's test addition. No payment-adjacent endpoint exists in the inspected API, so no payment test gap can be created without inventing functionality.

## Tests Added This Day

| Test | Gap closed | Expected behavior |
|---|---|---|
| `Login_WithInvalidPassword_ReturnsUnauthorized` | `POST /api/auth/login` invalid credentials | Returns `401 Unauthorized`. |
| `PatientCannotReadAnotherPatientsMedication` | `GET /api/medications/{id}` ownership denial | A Patient token cannot read another patient's medication; returns `403 Forbidden`. |
| `PatientCannotReadAnotherPatientsAppointment` | `GET /api/appointments/{id}` ownership denial | A Patient token cannot read another patient's appointment; returns `403 Forbidden`. |
| `PatientCreatingInvalidVitalSignReceivesBadRequest` | `POST /api/patients/{patientId}/vital-signs` validation | An authenticated owner with invalid blood-pressure ordering receives `400 Bad Request`. |
