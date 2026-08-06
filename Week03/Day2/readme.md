# Day 2 – SQL Server Schema Design & Normalization

## Overview

This day focused on designing a well-structured relational database schema using SQL Server normalization principles. The goal was to eliminate data redundancy, improve data consistency, and build clear relationships between tables.

## Learning Objectives

* Understand why database normalization is important.
* Apply First, Second, and Third Normal Forms (1NF, 2NF, 3NF).
* Design primary keys and foreign keys.
* Create relationships between database tables.
* Select appropriate SQL Server data types.

## Hands-On Lab

The lab included the following tasks:

* Identified the required database entities and their attributes.
* Applied 1NF, 2NF, and 3NF to remove redundancy.
* Defined primary and foreign key relationships.
* Created an Entity Relationship Diagram (ERD).
* Selected appropriate data types for every column, using `DECIMAL(10,2)` for monetary values.

## Database Entities

* Customers
* Products
* Orders
* OrderItems

## Relationships

* One Customer → Many Orders
* One Order → Many OrderItems
* One Product → Many OrderItems

## Key Concepts Learned

* Database normalization reduces duplicated data and prevents update anomalies.
* Primary keys uniquely identify each record.
* Foreign keys enforce relationships and maintain referential integrity.
* Proper data type selection improves storage efficiency and data accuracy.
* Monetary values should use `DECIMAL` instead of `FLOAT`.

## Outcome

Designed a normalized SQL Server database schema with clear relationships, proper key constraints, and suitable data types, providing a strong foundation for building scalable RESTful APIs.
