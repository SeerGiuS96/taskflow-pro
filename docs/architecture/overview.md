# System Architecture Overview

## The Big Picture

TaskFlow Pro is split into two independent applications that communicate over HTTP and WebSockets:

```mermaid
graph TB
    subgraph Client["Browser"]
        Angular["Angular 19 SPA<br/>NgRx · Material · CDK"]
    end

    subgraph API["Backend (.NET 9)"]
        Controller["API Controllers"]
        MediatR["MediatR Pipeline<br/>(Validation · Auth · Logging)"]
        Handlers["Command & Query Handlers"]
        Domain["Domain Layer<br/>(Entities · Events · Rules)"]
        EF["Entity Framework Core 9"]
        SignalR["SignalR Hub"]
    end

    subgraph Data["Data Layer"]
        PG["PostgreSQL 16<br/>(primary data)"]
        Redis["Redis<br/>(cache · SignalR backplane)"]
        MinIO["MinIO<br/>(file storage)"]
    end

    Angular -->|"HTTPS REST"| Controller
    Angular <-->|"WebSocket"| SignalR
    Controller --> MediatR
    MediatR --> Handlers
    Handlers --> Domain
    Handlers --> EF
    EF --> PG
    SignalR --> Redis
    Handlers -->|"files"| MinIO
```

---

## Request Lifecycle (Backend)

Every API request follows the same path through the system:

```mermaid
sequenceDiagram
    participant C as Controller
    participant P as MediatR Pipeline
    participant V as ValidationBehavior
    participant A as AuthorizationBehavior
    participant L as LoggingBehavior
    participant H as Handler
    participant DB as Database

    C->>P: Send(CreateTaskCommand)
    P->>V: Validate inputs
    V-->>P: Valid ✓
    P->>A: Check permissions
    A-->>P: Authorized ✓
    P->>L: Log start
    P->>H: Execute handler
    H->>DB: Save changes
    DB-->>H: Task saved
    H-->>P: Result<TaskDto>
    P->>L: Log duration
    P-->>C: Result<TaskDto>
    C-->>C: Return 201 Created
```

**Key insight:** The handler only handles business logic. Everything else (validation, auth, logging) is handled by the pipeline automatically for every command.

---

## The Four Layers

```mermaid
graph LR
    API["🌐 API Layer<br/>Controllers<br/>Middleware<br/>Filters"]
    APP["⚙️ Application Layer<br/>Commands & Queries<br/>Handlers<br/>DTOs"]
    DOM["💎 Domain Layer<br/>Entities<br/>Domain Events<br/>Business Rules"]
    INF["🔧 Infrastructure Layer<br/>EF Core<br/>JWT<br/>Storage<br/>Email"]

    API --> APP
    APP --> DOM
    INF --> APP
    INF --> DOM
```

### Dependency Rule
**The dependency always points inward.** Domain knows nothing about Entity Framework. Application knows nothing about HTTP. This is what makes the code testable and maintainable.

| Layer | Knows about | Does NOT know about |
|---|---|---|
| Domain | Nothing external | EF Core, HTTP, SignalR |
| Application | Domain only | EF Core, HTTP |
| Infrastructure | Domain + Application | HTTP controllers |
| API | All layers | — |

---

## Multi-Tenancy Strategy

Every organization's data is isolated at the **query level** using EF Core Global Query Filters:

```mermaid
graph TD
    Request["HTTP Request<br/>GET /api/projects"] --> Middleware["TenantResolutionMiddleware<br/>reads org slug from route"]
    Middleware --> Context["ITenantContext<br/>OrganizationId = 'abc-123'"]
    Context --> EF["EF Core DbContext<br/>Global Query Filter applied"]
    EF --> Query["SELECT * FROM Projects<br/>WHERE organization_id = 'abc-123'<br/>(filter added automatically)"]
```

A developer cannot forget to filter by tenant — EF Core adds the `WHERE organization_id = ?` clause automatically to every query.

---

## Real-time Architecture

```mermaid
sequenceDiagram
    participant UserA as User A (moves task)
    participant API as .NET API
    participant DomainEvent as Domain Event
    participant SignalR as SignalR Hub
    participant Redis as Redis PubSub
    participant UserB as User B (same board)

    UserA->>API: PATCH /tasks/123/move
    API->>API: Save to DB
    API->>DomainEvent: TaskMovedEvent fires
    DomainEvent->>SignalR: Push "task-moved" to board group
    SignalR->>Redis: Publish to backplane
    Redis->>SignalR: Broadcast to all instances
    SignalR->>UserB: Receives "task-moved" event
    UserB->>UserB: NgRx dispatches action, UI updates
```

---

## Further Reading

- [Backend Architecture](backend.md) — Clean Architecture layers in detail
- [Frontend Architecture](frontend.md) — Angular modules and NgRx data flow
- [Database ERD](../database/erd.md) — Full entity-relationship diagram
