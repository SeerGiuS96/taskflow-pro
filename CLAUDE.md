# CLAUDE.md — Project Context for Claude Code

This file is read automatically by Claude Code at the start of every session.
It keeps context across conversations so we never lose progress.

---

## The Project

**TaskFlow Pro** — a full-stack project management platform (like Linear/Jira).
Built as a personal learning project to become a senior developer and change jobs.

- **Backend:** .NET 9, Clean Architecture, CQRS with MediatR, PostgreSQL, SignalR
- **Frontend:** Angular 19, NgRx, Angular Material, Tailwind CSS
- **Infra:** Docker, GitHub Actions CI/CD, MinIO

Full documentation in `docs/`. Start with `docs/architecture/overview.md`.

---

## Developer Profile

- **Level:** Mid-level developer. Has real work experience.
- **Gaps:** Architecture patterns, Angular advanced, .NET Core, LINQ, NgRx — all being learned through this project.
- **Goal:** Become senior-level, change company, build an impressive portfolio project.
- **Language preference:** Explanations in Spanish. Code and docs in English.

---

## Current Phase

**Phase 0 — COMPLETED**
- Project structure created
- Full documentation written (architecture, ERD, ADRs, learning notes templates, roadmap)
- Git initialized, initial commit made

**Next: Phase 1 — Backend Skeleton + Auth**
See `docs/ROADMAP.md` for full phase details and checklist.

---

## How We Work Together

1. **Explain before implementing.** Before writing code for a new concept, explain it in simple terms.
2. **Learning notes.** After each concept is implemented, prompt the developer to fill in their `docs/learning-notes/` file in their own words.
3. **Commit after each meaningful unit.** Conventional Commits format with emoji. See README.md commit convention table.
4. **No skipping phases.** Each phase has a "ready when" criteria. Don't advance until criteria is met.
5. **The developer must understand everything.** If something is unclear, stop and explain. The goal is learning, not just shipping.

---

## Commit Convention

Format: `<emoji> <type>(<scope>): <description>`

| Emoji | Type | Use for |
|---|---|---|
| ✨ | feat | New feature |
| 🐛 | fix | Bug fix |
| ♻️ | refactor | Refactor |
| 📝 | docs | Documentation |
| ✅ | test | Tests |
| 🔧 | chore | Config, tooling |
| 🚀 | ci | CI/CD pipeline |
| 💄 | style | UI/styling |
| ⚡️ | perf | Performance |
| 🗃️ | db | Database/migrations |

---

## Key Files

| File | Purpose |
|---|---|
| `docs/ROADMAP.md` | Development phases and progress checklist |
| `docs/architecture/overview.md` | System architecture with Mermaid diagrams |
| `docs/architecture/backend.md` | Clean Architecture, CQRS, Domain Events |
| `docs/architecture/frontend.md` | Angular, NgRx, optimistic updates |
| `docs/database/erd.md` | Full database schema |
| `docs/adr/README.md` | Index of all Architecture Decision Records |
| `docs/learning-notes/README.md` | Index of personal study notes |

---

## Technical Decisions (summary)

- Clean Architecture: 4 projects (Domain, Application, Infrastructure, API)
- CQRS via MediatR with pipeline behaviors (validation, auth, logging, performance)
- PostgreSQL with EF Core 9 and Npgsql
- Multi-tenancy via EF Core global query filters
- JWT (15min) + refresh token rotation (7 days, HttpOnly cookie)
- NgRx for board state (optimistic updates + rollback)
- SignalR for real-time board sync
- Docker Compose for local dev
- GitHub Actions for CI/CD

Full reasoning in `docs/adr/`.

---

## Notes for Claude

- Always explain WHY before HOW
- When introducing a new pattern, show a simple example first, then apply to this project
- Remind the developer to fill learning notes after each phase
- Keep explanations in Spanish, code in English
- This project is meant to be shown in job interviews — quality over speed
