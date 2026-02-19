# ADR 002 — Use CQRS with MediatR

**Status:** Accepted
**Date:** 2025

---

## Context

We need a pattern for handling application operations (create task, get board, move task, etc.).

Options considered:
- **Service classes (TaskService, ProjectService):** Simple, everyone knows it. But services grow large over time. A `TaskService` with 20 methods is hard to navigate and test.
- **CQRS with MediatR (chosen):** Each operation is its own small class. The handler for CreateTask only contains CreateTask logic. Easy to find, easy to test independently.

---

## Decision

Use **CQRS** (Command Query Responsibility Segregation) implemented via **MediatR**:

- **Commands** — operations that change state (CreateTask, MoveTask, DeleteProject)
- **Queries** — operations that read state (GetTaskById, SearchTasks)
- Each command/query is a record (plain data)
- Each has its own handler class
- MediatR routes the message to the correct handler

Also use the **MediatR Pipeline** for cross-cutting concerns:
- `ValidationBehavior` — runs FluentValidation before every handler
- `AuthorizationBehavior` — checks org membership before every handler
- `LoggingBehavior` — logs every request automatically
- `PerformanceBehavior` — warns when a request takes over 500ms

---

## Consequences

**Good:**
- Each handler is a small, focused class — easy to read and test
- Cross-cutting concerns are written once and apply everywhere via the pipeline
- Adding a new feature = creating a new folder with Command + Handler + Validator
- No "fat service" problem
- Interview gold: CQRS is one of the most discussed patterns in senior .NET roles

**Tradeoffs:**
- More ceremony for simple CRUD — a simple "get user by id" needs 3 files (Query, Handler, Validator)
- MediatR adds a small overhead per request (negligible in practice)
- Developers need to understand the pattern to be productive

---

## Interview talking point

*"CQRS with MediatR gives me a pipeline of behaviors for cross-cutting concerns. Validation, authorization, logging, and performance monitoring all run automatically for every request without any code in the handlers themselves. Adding a new concern means adding one new behavior class — I don't touch any existing code."*
