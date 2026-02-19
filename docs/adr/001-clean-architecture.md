# ADR 001 — Use Clean Architecture for the backend

**Status:** Accepted
**Date:** 2025

---

## Context

We need to structure the .NET backend in a way that is:
- Testable (we can unit test business logic without a database)
- Maintainable (changing the database or framework doesn't break business rules)
- Clear (a new developer knows where to put new code)

The alternative approaches considered were:
- **Traditional N-Layer (UI → Business → Data):** Simple to start, but business logic often leaks into data layer or vice versa. Hard to test without a real database.
- **Vertical Slice Architecture:** Organizes by feature, not layer. Good alternative, but harder to learn for someone new to architecture concepts.
- **Clean Architecture (chosen):** Organizes in concentric layers. Inner layers have zero dependencies on outer layers. Business logic is pure C# — no EF Core, no HTTP.

---

## Decision

Use **Clean Architecture** with four projects:

1. `TaskFlowPro.Domain` — Entities, Domain Events, business rules. Zero external dependencies.
2. `TaskFlowPro.Application` — Use cases (CQRS handlers), DTOs, interfaces. Depends only on Domain.
3. `TaskFlowPro.Infrastructure` — EF Core, JWT, storage, email. Implements interfaces from Application.
4. `TaskFlowPro.API` — HTTP controllers, middleware. Entry point.

The **Dependency Rule:** source code dependencies only point inward. Nothing in Domain knows about EF Core. Nothing in Application knows about HTTP.

---

## Consequences

**Good:**
- Domain and Application layers are 100% unit-testable without starting a database
- Swapping PostgreSQL for another database only requires changes in Infrastructure
- Developers always know exactly where to put new code
- Widely used in the .NET ecosystem — familiar to other developers

**Tradeoffs:**
- More initial setup than a simple MVC project
- More files and folders — can feel like overkill for very small apps
- Requires discipline to not break the dependency rule as the project grows

---

## Interview talking point

*"I used Clean Architecture to ensure my business logic has zero dependency on infrastructure. I can run all my use case tests without a running database, which makes the test suite fast and reliable. The Domain project compiles without Entity Framework even being installed."*
