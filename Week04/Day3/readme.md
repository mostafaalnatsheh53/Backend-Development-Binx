# Day 3 — Authorization & Role-Based Access Control 🛡️

## Overview

Day 3 focused on protecting API endpoints and controlling access based on user roles and permissions.

## What I Learned

* 🛡️ Using `[Authorize]` to protect endpoints
* 👤 Role-Based Access Control (RBAC)
* 🔐 `User` and `Admin` roles
* 📋 Claims-Based and Policy-Based Authorization
* 🚫 Difference between `401 Unauthorized` and `403 Forbidden`

## Implementation

* Protected API endpoints using `[Authorize]`
* Created `User` and `Admin` roles
* Assigned roles to users using ASP.NET Core Identity
* Restricted the `Delete` endpoint to the `Admin` role
* Configured a `CanManageOrders` authorization policy
* Tested protected endpoints with JWT Bearer tokens in Postman

## Testing

Tested:

* Requests without a token → `401 Unauthorized`
* Authenticated users without permission → `403 Forbidden`
* Admin users accessing restricted endpoints → Allowed

## Technologies

* C#
* ASP.NET Core
* ASP.NET Core Identity
* JWT Bearer Authentication
* Role-Based Authorization
* Policy-Based Authorization
* Postman

## Key Takeaway

Implemented authorization on top of JWT authentication, ensuring that users can only access API operations allowed by their roles and permissions.
