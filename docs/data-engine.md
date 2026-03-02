# DataEngine

**DataEngine**e is a lightweight, structured **Obejct Relational Mapping (ORM)** library designed for the BuildHub ecosystem. 
It provides explicit database interaction, entity mapping, query construction, connection pooling, and transaction management.

## Overview

**DataEngine** centralizes and standardizes database operations across the system by providing:

- Database configuration management
- Connection pooling and validation
- Attribute-based entity mapping
- Fluent query building
- Scoped transaction handling
- Structured exception management

### 1. How to configure

The configuration of the **DataEngine** library is done throght combination of the `appsetings.json` and environment variables. Where in the environment variables the connections strings are stored:


**Example configuration**:
```json
"DatabaseConfigurations": [
    {
      "DatabaseSource": "Users",
      "MaxPoolConnections": 10,
      "MinPoolConnections": 5
    }
  ],
```

**Example Envirement Variable**
```
ConnectionStrings__BuildHubCore

Data Source=SERVER_NAME;Integrated Security=True;Initial Catalog=BuildHubCore;User ID=exampleUser;Password=examplePassword;TrustServerCertificate=True
```

```Note:The prefix 'ConnectionStrings__' is mandatory for the variable to work ```

---
## Role in BuildHub

Within BuildHub, **DataEngine** serves as the foundational data layer and powers:

- Core database operations
- Configuration persistence
- Execution plan storage
- Internal system state management

It acts as the backbone of backend data integrity.

## License
This library is part of the BuildHub project.  
Refer to the main repository for licensing details.
