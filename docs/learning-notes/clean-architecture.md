# Clean Architecture — Learning Notes

## The Problem It Solves

Imagine a typical project after 2 years without architecture:

```mermaid
graph TD
    C["Controller"] -->|"calls directly"| S["Service"]
    S -->|"uses"| EF["Entity Framework"]
    S -->|"uses"| HTTP["HttpClient"]
    S -->|"uses"| EMAIL["EmailService"]
    C -->|"also calls"| EF
    EF -->|"depends on"| DB["Database"]

    style C fill:#ff6b6b
    style S fill:#ff6b6b
    style EF fill:#ff6b6b
```

Problems:
- To test the business logic, you need a real database running
- To change from EF Core to Dapper, you touch 30 files
- To understand what `TaskService.cs` does, you have to read 800 lines
- Developers don't know where to put new code

**Clean Architecture solves this by enforcing a strict dependency rule.**

---

## The Dependency Rule

**Dependencies only point inward. Never outward.**

```mermaid
graph TD
    subgraph outer["Outer Layers"]
        subgraph middle["Middle Layer"]
            subgraph inner["Inner Layer"]
                D["Domain\nEntities · Rules · Events\n\nDepends on: nothing"]
            end
            A["Application\nUse Cases · CQRS Handlers\n\nDepends on: Domain only"]
        end
        I["Infrastructure\nEF Core · JWT · Email · Storage\n\nDepends on: Application + Domain"]
        API["API\nControllers · Middleware\n\nDepends on: Infrastructure"]
    end
```

The Domain layer compiles with zero NuGet packages. It is pure C#.

---

## The 4 Layers Explained

### Layer 1: Domain
**What lives here:** Entities, Domain Events, business rules, interfaces that Infrastructure must implement.

**Key rule:** Zero external dependencies. No EF Core. No HTTP. Nothing.

```csharp
// Domain/Entities/User.cs
// No "using Microsoft.EntityFrameworkCore" here — ever
public class User : BaseEntity
{
    public string Email { get; private set; }

    public void SetRefreshToken(string hash, DateTime expiresAt)
    {
        RefreshTokenHash = hash;
        RefreshTokenExpiresAt = expiresAt;
    }
}
```

### Layer 2: Application
**What lives here:** CQRS commands and queries, handlers, DTOs, validators, interfaces for services.

**Key rule:** Depends only on Domain. Talks to the database through an `IApplicationDbContext` interface defined here and implemented in Infrastructure.

```csharp
// Application defines the interface...
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}

// Infrastructure implements it
public class ApplicationDbContext : DbContext, IApplicationDbContext { ... }
```

### Layer 3: Infrastructure
**What lives here:** EF Core, JWT, file storage, email, SignalR, any external system.

**Key rule:** Implements interfaces from Application. The rest of the system never knows what is behind those interfaces.

### Layer 4: API
**What lives here:** Controllers, Middleware, Program.cs.

**Key rule:** Thin layer. Controllers just receive HTTP, send a MediatR command, return the result. Zero business logic.

---

## Why This Matters: Testing

```mermaid
graph TD
    subgraph A["Without Clean Architecture"]
        T1["Test RegisterUser()"] -->|"needs"| DB1["Real PostgreSQL running"]
        T1 -->|"needs"| SMTP["Real SMTP server"]
    end

    subgraph B["With Clean Architecture"]
        T2["Test RegisterUserHandler()"] -->|"uses"| MOCK["Mocked IApplicationDbContext\nin-memory, no DB needed"]
        T2 -->|"uses"| MOCK2["Mocked IEmailService\nno real emails sent"]
    end
```

Unit tests run in milliseconds with no external dependencies. You mock the interfaces. The handler only sees the interface — it cannot tell if it is talking to PostgreSQL or a fake in-memory implementation.

---

## The Flow of a Request

```mermaid
sequenceDiagram
    participant HTTP as HTTP Request
    participant C as AuthController (API)
    participant M as MediatR
    participant VB as ValidationBehavior (Application)
    participant H as RegisterCommandHandler (Application)
    participant DB as ApplicationDbContext (Infrastructure)
    participant PG as PostgreSQL

    HTTP->>C: POST /auth/register
    C->>M: Send(new RegisterCommand)
    M->>VB: Validate inputs
    VB-->>M: Valid
    M->>H: Handle(RegisterCommand)
    H->>DB: Check if email exists
    DB->>PG: SELECT COUNT(*) FROM users WHERE email = $1
    PG-->>H: 0 (does not exist)
    H->>H: Hash password, create User entity
    H->>DB: Users.Add(user), SaveChangesAsync()
    DB->>PG: INSERT INTO users
    H-->>C: Result.Success(userDto)
    C-->>HTTP: 201 Created
```

The Controller knows nothing about the database. The Handler knows nothing about HTTP. Each layer only does its job.

---

## Project References Enforce the Rule at Compile Time

```mermaid
graph LR
    API -->|"references"| INF["Infrastructure"]
    API -->|"references"| APP["Application"]
    INF -->|"references"| APP
    INF -->|"references"| DOM["Domain"]
    APP -->|"references"| DOM

    DOM -. "cannot reference" .-> APP
    DOM -. "cannot reference" .-> INF
    APP -. "cannot reference" .-> INF
```

If you try to use EF Core in the Domain project, it will not compile. There is no package reference for it. The architecture is enforced by the compiler.

---

## Common Mistakes

| Mistake | Why it breaks the rule |
|---|---|
| Calling DbContext from a Controller | Controller bypasses Application layer |
| Importing HttpContext in a Handler | Application layer depends on HTTP infrastructure |
| Putting `[Required]` on a Domain entity | Domain depends on a framework annotation |
| Business logic inside a Controller | Logic that should be testable is hidden in HTTP layer |

---

## Interview Questions

**"Can you explain Clean Architecture?"**
> "It organizes code in layers where dependencies only point inward. The Domain layer has zero external dependencies — it is pure C#. Application contains use cases and depends only on Domain. Infrastructure implements interfaces defined in Application. This means business logic is testable without a running database, and you can swap implementations without changing business rules."

**"Why use 4 projects instead of one?"**
> "Project references enforce architectural boundaries at compile time. If everything is in one project, nothing stops a developer from using DbContext directly in a controller. Separate projects make violations impossible to build."

**"What is the Dependency Rule?"**
> "Dependencies only point inward. Domain knows nothing about EF Core. Application knows nothing about HTTP. This is enforced at compile time — the Domain project cannot import Entity Framework because there is no package reference for it."

---

## My Notes (write in your own words)

> After building Phase 1 and 2, come back and fill this in:
>
> - Before Clean Architecture, where did I put business logic?
> - What is the hardest habit to break?
> - My own analogy for the dependency rule:
