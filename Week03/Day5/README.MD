# Week 3 - Postman Collection & API Testing

This repository contains the Week 3 backend practice for the BinX Backend Internship. The project focuses on building and testing RESTful API endpoints using ASP.NET Core and Postman.

---

## Features

- RESTful API implementation
- CRUD operations for Orders
- SQL Server with Entity Framework Core
- Postman Collection for API testing
- Happy Path and Error Path testing
- Automated Postman Test Scripts
- Environment Variables using `baseUrl`

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{id}` | Get an order by ID |
| POST | `/api/orders` | Create a new order |
| PUT | `/api/orders/{id}` | Update an existing order |
| DELETE | `/api/orders/{id}` | Delete an order |

---

## Postman Collection

The Postman collection includes:

- Happy Path requests
- Error Path requests
- Automated status code tests
- Environment variables

Example variable:

```text
{{baseUrl}} = http://localhost:5138
```

---

## Testing

Implemented automated Postman test scripts to verify expected HTTP status codes.

Examples:

- 200 OK
- 201 Created
- 204 No Content
- 400 Bad Request
- 404 Not Found

---

## Technologies Used

- ASP.NET Core
- C#
- Entity Framework Core
- SQL Server
- Postman

---

## Project Structure

```
Day5/
│
├── Orders
│   ├── Happy Path
│   └── Error Path
│
├── Week3_Postman_Collection_Report.pdf
└── README.md
```

---

## Author

**Mostafa Alnatsheh**

Backend Development Internship – BinX Tech