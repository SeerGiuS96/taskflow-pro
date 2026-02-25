# Learning Notes

Personal reference notes built while developing TaskFlow Pro.
Each note covers one concept — with diagrams, code from this project, and interview Q&A.

## How to use

- Read a note **before** implementing the concept — get the mental model first
- Read it again **after** implementing — fill in the "My Notes" section with your own words
- Review all notes before job interviews

---

## Index by Phase

### Phase 1 — Backend Foundation

| File | Concept | Status |
|---|---|---|
| [clean-architecture.md](clean-architecture.md) | The 4 layers, dependency rule, why it matters | ✅ Written |
| [ef-core.md](ef-core.md) | ORM, DbContext, Fluent API, migrations, Change Tracker | ✅ Written |
| [jwt-auth.md](jwt-auth.md) | JWT tokens, refresh rotation, storage strategy | ⏳ Fill after implementing auth |

### Phase 2 — CQRS Core

| File | Concept | Status |
|---|---|---|
| [cqrs-mediatr.md](cqrs-mediatr.md) | Commands vs Queries, MediatR, Pipeline Behaviors | ⏳ Fill after implementing first handler |
| [result-pattern.md](result-pattern.md) | Result\<T\> — returning errors without exceptions | ✅ Written |

### Phase 3 — Multi-Tenancy

| File | Concept | Status |
|---|---|---|
| [multi-tenancy.md](multi-tenancy.md) | Global query filters, tenant isolation | ⏳ Fill in Phase 3 |

### Phase 4-5 — Frontend

| File | Concept | Status |
|---|---|---|
| [ngrx.md](ngrx.md) | Store, Actions, Reducers, Effects, Selectors | ⏳ Fill in Phase 4 |
| [optimistic-updates.md](optimistic-updates.md) | Optimistic UI + rollback pattern | ⏳ Fill in Phase 5 |

### Phase 6 — Events & Real-time

| File | Concept | Status |
|---|---|---|
| [domain-events.md](domain-events.md) | Domain events, decoupled side effects | ⏳ Fill in Phase 6 |
| [signalr.md](signalr.md) | Real-time with SignalR, hub, Angular client | ⏳ Fill in Phase 6 |

### Phase 7-8 — Quality & DevOps

| File | Concept | Status |
|---|---|---|
| [testing.md](testing.md) | Unit tests, integration tests, Testcontainers | ⏳ Fill in Phase 7 |
| [docker.md](docker.md) | Docker, Docker Compose, multi-stage builds | ⏳ Fill in Phase 8 |
| [github-actions.md](github-actions.md) | CI/CD pipelines, automated testing on PR | ⏳ Fill in Phase 8 |

---

## Session Notes (chronological)

| File | Session | Topics covered |
|---|---|---|
| [session-01-concepts.md](session-01-concepts.md) | Session 01 | CQRS, MediatR, multi-tenancy, encapsulation, Clean Architecture intro |

---

## Note Structure

Every note follows this pattern:

1. **The problem it solves** — why does this exist?
2. **The concept explained simply** — no jargon first
3. **How it works in this project** — concrete code examples
4. **How it works in general** — applicable to any project
5. **Diagrams** — visual representation (Mermaid)
6. **Interview Q&A** — ready-made answers
7. **My Notes** — YOUR section, in your own words
