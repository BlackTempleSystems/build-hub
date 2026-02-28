# DataEngine

DataEngine is a lightweight, structured data-access library designed for the BuildHub ecosystem.  
It provides explicit database interaction, entity mapping, query construction, connection pooling, and transaction management — without relying on heavy ORM abstractions.

The goal is clarity, control, performance, and predictable behavior.

---

## Overview

DataEngine centralizes and standardizes database operations across the system by providing:

- Database configuration management
- Connection pooling and validation
- Attribute-based entity mapping
- Fluent query building
- Scoped transaction handling
- Structured exception management
- Internal statistics tracking

It favors explicit behavior over implicit “magic”.

---

## Architecture

### 1. Configuration

Responsible for registering and validating database connection settings.

**Main Components**
- `DatabaseConfiguration`
- `DatabaseConfigurationManager`

**Responsibilities**
- Manage connection settings
- Validate configuration integrity
- Provide controlled access to connection metadata

---

### 2. Connection Management

Handles safe and efficient database connections.

**Main Components**
- `DatabaseConnection`
- `DatabaseConnectionPool`
- `DatabaseContext`
- `DatabaseConnectionValidator`
- `DatabaseSource`

**Features**
- Connection pooling
- Leak detection
- Connection validation
- Controlled lifecycle management

---

### 3. Entity Mapping

DataEngine uses attribute-based mapping instead of a full ORM.

**Main Components**
- `BaseEntity`
- `VersionedEntity`
- `EntityDataMapper`
- `TableNameAttribute`
- `PrimaryKeyAttribute`
- `IdentityAttribute`
- `ColumnDescriptionAttribute`

**Features**
- Explicit table mapping
- Strongly typed entities
- Optimistic concurrency support
- Clear schema-to-model alignment

---

### 4. Query Builder

Provides a fluent and controlled API for constructing SQL queries.

**Main Components**
- `QueryBuilder`
- `InternalQueryBuilder`
- `WhereCondition`
- `CompareTypes`
- `LockTypes`

**Features**
- Fluent query construction
- Controlled query lifecycle
- Prevention of invalid execution states
- Explicit locking behavior
- Defensive validation

---

### 5. Transactions

Supports safe and explicit transaction management.

**Main Components**
- `ScopedTransaction`
- `ITransactionContext`

**Features**
- Explicit transaction boundaries
- Safe commit/rollback handling
- Context-aware execution

---

### 6. Exception Handling

DataEngine uses a structured exception hierarchy grouped by domain:

- DatabaseConnection
- Entities
- Queries

This ensures precise error reporting and easier debugging.

---

### 7. Statistics & Observability

**Component**
- `DataEngineStatistics`

Provides internal metrics tracking for monitoring and diagnostics.

---

## Design Philosophy

DataEngine intentionally avoids:

- Heavy ORM abstraction layers
- Hidden SQL generation
- Implicit state changes
- Over-automated behavior

Instead, it emphasizes:

- Explicit configuration
- Controlled state transitions
- Clear separation of responsibilities
- Defensive validation
- Predictable execution flow

---

## When to Use DataEngine

Use DataEngine when:

- You need structured, reusable database access
- You want controlled SQL generation
- You require transaction safety
- You prefer explicit over implicit behavior
- You need connection pooling without a full ORM

Avoid using it for:

- Rapid prototyping with dynamic schema generation
- Complex object graph mapping typical of full ORMs

---

## Role in BuildHub

Within BuildHub, DataEngine serves as the foundational data layer and powers:

- Core database operations
- Configuration persistence
- Execution plan storage
- Internal system state management

It acts as the backbone of backend data integrity.

---

## License

This library is part of the BuildHub project.  
Refer to the main repository for licensing details.
