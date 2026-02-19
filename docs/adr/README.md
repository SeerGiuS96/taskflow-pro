# Architecture Decision Records (ADRs)

ADRs document **why** we made architectural decisions, not just what we decided.
In interviews, being able to explain the reasoning behind decisions is what separates senior from mid-level engineers.

## Index

| # | Decision | Status |
|---|---|---|
| [001](001-clean-architecture.md) | Use Clean Architecture for the backend | Accepted |
| [002](002-cqrs-mediatr.md) | Use CQRS with MediatR | Accepted |
| [003](003-postgresql.md) | Use PostgreSQL as primary database | Accepted |
| [004](004-ngrx-state-management.md) | Use NgRx for Angular state management | Accepted |
| [005](005-jwt-refresh-tokens.md) | Use JWT with refresh token rotation | Accepted |
| [006](006-docker-local-dev.md) | Use Docker Compose for local development | Accepted |
| [007](007-mermaid-diagrams.md) | Use Mermaid for architecture diagrams | Accepted |

## ADR Format

Each ADR follows this structure:
- **Context** — Why did we need to make a decision?
- **Decision** — What did we decide?
- **Consequences** — What does this mean going forward? What are the tradeoffs?
