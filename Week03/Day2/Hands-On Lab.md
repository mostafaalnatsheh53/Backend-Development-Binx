# Day 2 – SQL Server Schema Design & Normalization

## Hands-On Lab: Design & Diagram a Normalized Schema

### 1. Entities and Attributes

#### Customers

* Id
* Name
* Email

#### Products

* Id
* Name
* Price

#### Orders

* Id
* CustomerId
* OrderDate
* Total

#### OrderItems

* Id
* OrderId
* ProductId
* Quantity

---

## 2. Apply 1NF, 2NF, and 3NF

### First Normal Form (1NF)

* Stored one value per column (atomic values).
* Moved products into a separate `OrderItems` table.

### Second Normal Form (2NF)

* Stored customer and product information in their own tables.
* Removed partial dependencies.

### Third Normal Form (3NF)

* Customer details are stored only in the `Customers` table.
* Product details are stored only in the `Products` table.
* The `Orders` table stores only `CustomerId` instead of duplicating customer information.

---

## 3. Primary Keys and Foreign Keys

### Primary Keys

* **Customers** → `Id`
* **Products** → `Id`
* **Orders** → `Id`
* **OrderItems** → `Id`

### Foreign Keys

* `Orders.CustomerId` → `Customers.Id`
* `OrderItems.OrderId` → `Orders.Id`
* `OrderItems.ProductId` → `Products.Id`

---

## 4. ERD (Database Diagram)

The following DBML schema was used to generate the ERD:

```dbml
Table Customers {
  Id int [pk, increment]
  Name nvarchar(100)
  Email varchar(255)
}

Table Products {
  Id int [pk, increment]
  Name nvarchar(100)
  Price decimal(10,2)
}

Table Orders {
  Id int [pk, increment]
  CustomerId int
  OrderDate datetime
  Total decimal(10,2)
}

Table OrderItems {
  Id int [pk, increment]
  OrderId int
  ProductId int
  Quantity int
}

Ref: Orders.CustomerId > Customers.Id
Ref: OrderItems.OrderId > Orders.Id
Ref: OrderItems.ProductId > Products.Id
```

---

## 5. Column Types

| Attribute | Data Type     |
| --------- | ------------- |
| Id        | INT           |
| Name      | NVARCHAR(100) |
| Email     | VARCHAR(255)  |
| Price     | DECIMAL(10,2) |
| Quantity  | INT           |
| OrderDate | DATETIME      |
| Total     | DECIMAL(10,2) |

---

## Summary

In this lab, I designed a normalized SQL Server database schema by:

* Identifying the required entities and their attributes.
* Applying the 1NF, 2NF, and 3NF normalization rules.
* Defining primary and foreign key relationships.
* Creating an ERD using DBML.
* Selecting appropriate SQL Server data types, including `DECIMAL(10,2)` for monetary values.
