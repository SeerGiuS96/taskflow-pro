# Database - Entity Relationship Diagram

## Full ERD

```mermaid
erDiagram
    Organizations {
        uuid id PK
        varchar name
        varchar slug "unique, used in URLs"
        enum plan "free | pro | enterprise"
        timestamp created_at
        timestamp updated_at
    }

    Users {
        uuid id PK
        varchar email "unique"
        varchar password_hash
        varchar display_name
        varchar avatar_url
        bool is_email_verified
        varchar refresh_token_hash
        timestamp refresh_token_expires_at
        timestamp created_at
        timestamp updated_at
    }

    OrganizationMembers {
        uuid id PK
        uuid organization_id FK
        uuid user_id FK
        enum role "owner | admin | member | viewer"
        timestamp joined_at
        uuid invited_by_user_id FK
    }

    Projects {
        uuid id PK
        uuid organization_id FK
        varchar name
        text description
        varchar key "e.g. PROJ for PROJ-42"
        enum status "active | archived"
        enum visibility "public_to_org | private"
        uuid created_by_user_id FK
        timestamp created_at
        timestamp updated_at
    }

    ProjectMembers {
        uuid id PK
        uuid project_id FK
        uuid user_id FK
        enum role "manager | contributor | viewer"
        timestamp added_at
    }

    Boards {
        uuid id PK
        uuid project_id FK
        varchar name
        bool is_default
    }

    BoardColumns {
        uuid id PK
        uuid board_id FK
        varchar name
        int order_index
        varchar color "hex color"
        bool is_done_column
    }

    Labels {
        uuid id PK
        uuid project_id FK
        varchar name
        varchar color
    }

    Tasks {
        uuid id PK
        uuid project_id FK
        uuid board_column_id FK
        int task_number "auto-increment per project"
        varchar title
        text description "markdown"
        enum priority "urgent | high | medium | low | none"
        enum status "open | in_progress | done | cancelled"
        uuid assignee_user_id FK
        uuid reporter_user_id FK
        date due_date
        decimal estimated_hours
        float order_index "float allows insert without rewrite"
        uuid parent_task_id FK "self-reference for subtasks"
        timestamp created_at
        timestamp updated_at
        timestamp completed_at
    }

    TaskLabels {
        uuid task_id FK
        uuid label_id FK
    }

    Comments {
        uuid id PK
        uuid task_id FK
        uuid user_id FK
        text content "markdown"
        bool is_edited
        uuid parent_comment_id FK "self-reference for threads"
        timestamp created_at
        timestamp updated_at
    }

    Attachments {
        uuid id PK
        uuid task_id FK
        uuid uploaded_by_user_id FK
        varchar file_name
        bigint file_size_bytes
        varchar content_type
        varchar storage_key "path in MinIO / S3"
        timestamp uploaded_at
    }

    AuditLogs {
        uuid id PK
        uuid organization_id FK
        uuid user_id FK
        varchar entity_type "Task, Project, etc."
        uuid entity_id
        varchar action "Created | Updated | Deleted | Moved"
        jsonb old_values "previous state snapshot"
        jsonb new_values "new state snapshot"
        varchar ip_address
        timestamp occurred_at
    }

    Notifications {
        uuid id PK
        uuid user_id FK
        varchar type
        varchar title
        text message
        varchar entity_type
        uuid entity_id
        bool is_read
        timestamp created_at
    }

    Organizations ||--o{ OrganizationMembers : "has members"
    Users ||--o{ OrganizationMembers : "belongs to"
    Organizations ||--o{ Projects : "owns"
    Projects ||--o{ ProjectMembers : "has members"
    Users ||--o{ ProjectMembers : "belongs to"
    Projects ||--o{ Boards : "has"
    Boards ||--o{ BoardColumns : "has columns"
    Projects ||--o{ Labels : "has"
    Projects ||--o{ Tasks : "contains"
    BoardColumns ||--o{ Tasks : "holds"
    Users ||--o| Tasks : "assigned to"
    Users ||--o{ Tasks : "reported by"
    Tasks ||--o{ Tasks : "subtasks"
    Tasks ||--o{ TaskLabels : "tagged with"
    Labels ||--o{ TaskLabels : "applied to"
    Tasks ||--o{ Comments : "has"
    Comments ||--o{ Comments : "threaded"
    Tasks ||--o{ Attachments : "has"
    Users ||--o{ Notifications : "receives"
    Organizations ||--o{ AuditLogs : "tracks"
```

---

## Design Decisions

### Why UUIDs instead of integers as primary keys?

- **Security:** Sequential integers (`/tasks/1`, `/tasks/2`) let attackers enumerate resources. UUIDs are unguessable.
- **Distributed systems:** If you ever split into microservices or sync data between systems, UUIDs don't clash. Integers do.
- **Import/export:** Moving data between environments doesn't cause ID conflicts.

**Downside:** Slightly larger storage, slightly slower index lookups. Acceptable tradeoff for most systems.

### Why `order_index` is a float?

If you store order as integers (1, 2, 3, 4) and you insert between 2 and 3, you have to renumber every item after position 2. With floats, inserting between 2 and 3 becomes 2.5. Between 2 and 2.5 becomes 2.25. You only renumber when precision runs out (very rarely).

### Why JSONB for `old_values` / `new_values` in AuditLogs?

The alternative is a separate audit table for every entity. That's 15+ tables just for audit history. With JSONB, one `audit_logs` table captures any entity's state as a snapshot. PostgreSQL's JSONB is indexed and queryable — you can `WHERE old_values->>'status' = 'open'`.

### Why `task_number` is a separate integer?

Users need human-readable task codes like `PROJ-42`. If you use the UUID as the task identifier in the UI, it's unusable. The `task_number` auto-increments per project (not globally), so each project has its own sequence: PROJ-1, PROJ-2... API-1, API-2...

### Why `refresh_token_hash` and not plain refresh token?

If someone gets read access to your database (SQL injection, backup leak), they get all refresh tokens and can impersonate every user. Storing the hash means the plain token is never at rest. Same principle as password hashing.
