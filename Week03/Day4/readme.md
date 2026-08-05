# Day 4 – Implementing CRUD Operations with EF Core

## Overview

On Day 4 of the BinX Backend Development Internship, I implemented complete CRUD (Create, Read, Update, Delete) operations using Entity Framework Core. The focus was on building RESTful API endpoints, working with asynchronous database operations, handling validation and error responses, and understanding EF Core change tracking.

---

## Learning Objectives

* Implement **Create** and **Read** operations using asynchronous EF Core queries.
* Implement **Update** and **Delete** operations with proper change tracking.
* Handle **404 Not Found** and **400 Bad Request** responses.
* Understand how **SaveChangesAsync()** and **Change Tracking** work in EF Core.
* Test REST API endpoints using Postman.

---

## Topics Covered

### Create Operation

* Created new records using `Add()`.
* Saved changes asynchronously using `SaveChangesAsync()`.
* Returned **201 Created** responses with `CreatedAtAction()` and a Location header.

### Read Operations

* Implemented **Get All** using `ToListAsync()`.
* Implemented **Get By Id** using `FirstOrDefaultAsync()`.
* Returned **404 Not Found** when a requested resource does not exist.

### Update Operation

* Retrieved an entity before modifying it.
* Updated tracked entity properties.
* Saved changes using `SaveChangesAsync()`.
* Returned **400 Bad Request** for invalid input.
* Returned **404 Not Found** when the resource was not found.

### Delete Operation

* Retrieved the entity before deletion.
* Removed the entity using `Remove()`.
* Returned **204 No Content** after successful deletion.
* Returned **404 Not Found** if the resource did not exist.

### Validation

* Used **DataAnnotations** for request validation.
* Checked `ModelState.IsValid` before processing requests.
* Returned clear **400 Bad Request** responses for invalid input.

### Change Tracking

* Learned how EF Core tracks entity changes automatically.
* Understood entity states:

  * Added
  * Modified
  * Deleted
* Learned when to use `AsNoTracking()` for read-only queries to improve performance.

---

## Hands-On Lab

Implemented a complete CRUD API for the primary resource:

* ✅ Create Order
* ✅ Get All Orders
* ✅ Get Order By Id
* ✅ Update Order
* ✅ Delete Order

Tested all endpoints using Postman, including successful requests and error scenarios.

---

## Technologies Used

* C#
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Swagger
* Postman

---

## Key Takeaways

* Built a complete RESTful CRUD API using EF Core.
* Applied asynchronous database operations with `async` and `await`.
* Improved API reliability with validation and proper HTTP status codes.
* Learned how EF Core Change Tracking optimizes database updates.
* Practiced testing API endpoints and handling common error cases.

---

## Repository

This day's implementation is available in the Day4 project folder as part of the BinX Backend Development Internship repository.
