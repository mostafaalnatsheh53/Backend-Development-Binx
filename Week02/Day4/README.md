# Day 4 - ASP.NET Core Web API

## Overview
This project was created as part of the BinX Backend Development Internship - Day 4.

The project demonstrates:
- Creating an ASP.NET Core Web API project.
- Working with Controllers.
- Creating Minimal APIs.
- Using Routes and Route Parameters.
- Using HTTP Verbs (GET).

## Project Structure

- **Controllers**
  - HelloController
  - StudentController
  - ProductController

- **Program.cs**
  - Registers services
  - Configures middleware
  - Maps Controllers
  - Contains Minimal API endpoints

## Endpoints

### Controllers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Hello` | Returns a simple greeting |
| GET | `/Student` | Returns a welcome message |
| GET | `/Product` | Returns all products |
| GET | `/Product/{id}` | Returns a product by ID |

### Minimal APIs

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/products` | Returns all products |
| GET | `/products/{id}` | Returns a product by ID |
| GET | `/weatherforecast` | Returns sample weather data |

## Technologies Used

- C#
- .NET 10
- ASP.NET Core Web API
- Minimal APIs
- Controllers
- OpenAPI

## How to Run

```bash
dotnet restore
dotnet run
```

The API will be available at:

```text
http://localhost:5104
```

## Learning Outcomes

- Understand ASP.NET Core Web API structure.
- Build Controllers and Minimal APIs.
- Use Routing and Route Parameters.
- Understand HTTP GET endpoints.
- Compare Controllers with Minimal APIs.