# Week 03 - Day 1: REST API Design Principles & Resource Modeling

## Overview

This project demonstrates the fundamental principles of designing RESTful APIs using ASP.NET Core. The focus is on creating well-structured endpoints, following REST conventions, using appropriate HTTP status codes, and applying API versioning.

---

## Learning Objectives

- Understand what makes an API RESTful.
- Apply REST resource naming conventions.
- Use HTTP methods correctly.
- Return appropriate HTTP status codes.
- Implement API versioning using URL segments.
- Design nested resources that represent ownership relationships.

---

## REST Resource

Primary Resource:

- Books

Nested Resource:

- Member Loans

---

## Implemented Endpoints

| Method | Endpoint | Description |
|---------|----------|-------------|
| GET | `/api/v1/books` | Retrieve all books |
| GET | `/api/v1/books/{id}` | Retrieve a specific book |
| POST | `/api/v1/books` | Create a new book |
| PUT | `/api/v1/books/{id}` | Update an existing book |
| DELETE | `/api/v1/books/{id}` | Delete a book |
| GET | `/api/v1/members/{id}/loans` | Retrieve all loans for a specific member |

---

## HTTP Status Codes Used

| Status Code | Purpose |
|-------------|---------|
| 200 OK | Successful GET and PUT requests |
| 201 Created | Resource created successfully |
| 204 No Content | Resource deleted successfully |
| 400 Bad Request | Invalid client input |
| 404 Not Found | Requested resource does not exist |

---

## API Versioning

This project uses **URL Versioning**.

Example:

```
/api/v1/books
```

Using versioning allows future API updates without breaking existing clients.

---

## Technologies Used

- ASP.NET Core Web API
- C#
- .NET 10
- Swagger (OpenAPI)

---

## Project Structure

```
Day1
│
├── Controllers
│   └── BooksController.cs
│
├── Program.cs
└── README.md
```

---

## Hands-On Lab

The following REST API design concepts were implemented:

- RESTful resource naming
- CRUD endpoints
- Nested resource endpoint
- Proper HTTP status codes
- API versioning

---

## Author

**Mostafa Al-Natsheh**
Backend Development Internship – BinX Tech