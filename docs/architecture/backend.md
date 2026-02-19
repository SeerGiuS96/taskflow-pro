# Backend Architecture

## Clean Architecture

The backend is organized in four projects following Clean Architecture (also called Onion Architecture):

```mermaid
graph TD
    subgraph Solution["TaskFlowPro Solution"]
        direction TB
        API["TaskFlowPro.API<br/>─────────────────<br/>Controllers<br/>Middleware<br/>Filters<br/>Program.cs"]
        INF["TaskFlowPro.Infrastructure<br/>─────────────────<br/>EF Core / DbContext<br/>JWT Token Service<br/>Storage Service<br/>Email Service<br/>SignalR Hub"]
        APP["TaskFlowPro.Application<br/>─────────────────<br/>Commands & Queries<br/>Handlers<br/>Validators<br/>MediatR Behaviors<br/>DTOs / Mappings"]
        DOM["TaskFlowPro.Domain<br/>─────────────────<br/>Entities<br/>Domain Events<br/>Value Objects<br/>Interfaces<br/>Enums"]
    end

    API -->|references| INF
    API -->|references| APP
    INF -->|references| APP
    INF -->|references| DOM
    APP -->|references| DOM
```

---

## CQRS with MediatR

Every operation in the system is either a **Command** (changes state) or a **Query** (reads state):

```mermaid
graph LR
    subgraph Commands["Commands (write)"]
        C1["CreateTaskCommand"]
        C2["MoveTaskCommand"]
        C3["AssignTaskCommand"]
        C4["DeleteProjectCommand"]
    end

    subgraph Queries["Queries (read)"]
        Q1["GetTaskByIdQuery"]
        Q2["GetBoardTasksQuery"]
        Q3["SearchTasksQuery"]
    end

    subgraph Handlers["Handlers"]
        H1["CreateTaskCommandHandler"]
        H2["MoveTaskCommandHandler"]
        H3["GetTaskByIdQueryHandler"]
    end

    C1 -->|MediatR routes| H1
    C2 -->|MediatR routes| H2
    Q1 -->|MediatR routes| H3
```

### What a Command looks like

```csharp
// The command - just data, no logic
public record CreateTaskCommand(
    Guid ProjectId,
    Guid BoardColumnId,
    string Title,
    TaskPriority Priority
) : IRequest<Result<TaskDto>>;

// The handler - all the logic lives here
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDto>>
{
    public async Task<Result<TaskDto>> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        // 1. Validate business rules
        // 2. Create domain entity
        // 3. Save to database
        // 4. Return result
    }
}
```

---

## MediatR Pipeline Behaviors

Before the handler runs, every command/query passes through a pipeline of behaviors:

```mermaid
graph LR
    REQ["Request"] --> VB
    VB["ValidationBehavior<br/>FluentValidation<br/>checks all inputs"] --> AB
    AB["AuthorizationBehavior<br/>checks org membership<br/>and role permissions"] --> LB
    LB["LoggingBehavior<br/>logs request name<br/>and user info"] --> PB
    PB["PerformanceBehavior<br/>measures duration<br/>warns if > 500ms"] --> H
    H["Handler<br/>(actual work)"] --> RES["Response"]
```

**Why this matters:** You write this logic ONCE and it applies to every command automatically. No copy-pasting validation logic into every controller.

---

## Domain Events

Domain Events decouple side effects from business logic:

```mermaid
sequenceDiagram
    participant H as MoveTaskHandler
    participant T as Task Entity
    participant DB as DbContext
    participant I as DomainEventInterceptor
    participant AH as AuditLogHandler
    participant NH as NotificationHandler

    H->>T: task.MoveTo(newColumn)
    T->>T: Adds TaskMovedEvent to domain events
    H->>DB: SaveChangesAsync()
    DB->>I: Interceptor fires after save
    I->>AH: Dispatch TaskMovedEvent
    I->>NH: Dispatch TaskMovedEvent
    AH->>DB: Write audit log record
    NH->>DB: Create notification for assignee
```

**Key insight:** `MoveTaskHandler` has zero knowledge of audit logs or notifications. Adding a new side effect (like sending an email) means creating a new event handler — the existing handler doesn't change.

---

## Result Pattern

Business rule failures use the Result pattern instead of exceptions:

```mermaid
graph TD
    H["Handler executes"] --> Check{"Business rule<br/>passes?"}
    Check -->|Yes| Success["Result.Success(taskDto)"]
    Check -->|No| Failure["Result.Failure(Error.TaskNotFound)"]
    Success --> API["API returns 200/201"]
    Failure --> API2["API returns 404/400/403"]
```

Exceptions are only for unexpected technical failures (database down, null reference, etc.), not for expected business scenarios like "task not found" or "user not authorized."

---

## Project Folder Structure

```
backend/
└── src/
    ├── TaskFlowPro.Domain/
    │   ├── Common/
    │   │   ├── BaseEntity.cs           # Id, CreatedAt, UpdatedAt
    │   │   ├── Result.cs               # Result<T> pattern
    │   │   └── IDomainEvent.cs
    │   ├── Entities/
    │   │   ├── Organization.cs
    │   │   ├── User.cs
    │   │   ├── Project.cs
    │   │   ├── Board.cs
    │   │   ├── BoardColumn.cs
    │   │   ├── Task.cs
    │   │   ├── Comment.cs
    │   │   └── AuditLog.cs
    │   ├── Events/
    │   │   ├── TaskCreatedEvent.cs
    │   │   ├── TaskMovedEvent.cs
    │   │   └── TaskAssignedEvent.cs
    │   └── Enums/
    │       ├── TaskPriority.cs
    │       ├── TaskStatus.cs
    │       └── OrganizationRole.cs
    │
    ├── TaskFlowPro.Application/
    │   ├── Common/
    │   │   ├── Behaviors/
    │   │   │   ├── ValidationBehavior.cs
    │   │   │   ├── AuthorizationBehavior.cs
    │   │   │   ├── LoggingBehavior.cs
    │   │   │   └── PerformanceBehavior.cs
    │   │   └── Interfaces/
    │   │       ├── IApplicationDbContext.cs
    │   │       └── ICurrentUserService.cs
    │   └── Features/
    │       ├── Auth/
    │       │   ├── Commands/Login/
    │       │   ├── Commands/Register/
    │       │   └── Commands/RefreshToken/
    │       ├── Tasks/
    │       │   ├── Commands/CreateTask/
    │       │   │   ├── CreateTaskCommand.cs
    │       │   │   ├── CreateTaskCommandHandler.cs
    │       │   │   └── CreateTaskCommandValidator.cs
    │       │   ├── Commands/MoveTask/
    │       │   └── Queries/GetTaskById/
    │       └── Projects/
    │
    ├── TaskFlowPro.Infrastructure/
    │   ├── Persistence/
    │   │   ├── ApplicationDbContext.cs
    │   │   ├── Configurations/          # EF Core Fluent API configs
    │   │   ├── Interceptors/
    │   │   │   ├── AuditableEntityInterceptor.cs
    │   │   │   └── DomainEventInterceptor.cs
    │   │   └── Migrations/
    │   ├── Identity/
    │   │   └── JwtTokenService.cs
    │   ├── Services/
    │   │   ├── CurrentUserService.cs
    │   │   └── TenantService.cs
    │   └── Hubs/
    │       └── NotificationHub.cs
    │
    └── TaskFlowPro.API/
        ├── Controllers/
        │   ├── AuthController.cs
        │   ├── ProjectsController.cs
        │   ├── TasksController.cs
        │   └── CommentsController.cs
        ├── Middleware/
        │   ├── ExceptionHandlingMiddleware.cs
        │   └── TenantResolutionMiddleware.cs
        └── Program.cs
```
