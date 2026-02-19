# ADR 003 — Use PostgreSQL as primary database

**Status:** Accepted
**Date:** 2025

---

## Context

We need a relational database. Options considered: SQL Server, MySQL, SQLite, PostgreSQL.

---

## Decision

Use **PostgreSQL 16**.

Key reasons:
- **JSONB support** — native JSON column type, indexed and queryable. Used for `old_values`/`new_values` in audit logs.
- **Free and open source** — no licensing cost, runs in Docker for free locally and cheaply in cloud.
- **Industry standard** — most cloud providers (AWS RDS, Azure, Railway, Supabase) support it natively.
- **Better than MySQL** for complex queries, window functions, and full-text search.
- **EF Core support** via Npgsql provider is excellent and well-maintained.

SQL Server was not chosen because it requires a license for production (or using the free tier with limitations). For a portfolio project and real production use, PostgreSQL is the better default.

SQLite was considered for development only but was ruled out to keep dev/prod parity from day one.

---

## Consequences

**Good:**
- No licensing cost ever
- JSONB for audit log — one flexible table instead of 15 entity-specific audit tables
- Runs in Docker with a single line in docker-compose.yml
- Testcontainers can spin up a real PostgreSQL instance for integration tests

**Tradeoffs:**
- Team members familiar with SQL Server need to learn minor PostgreSQL syntax differences
- Some SQL Server-specific EF Core features are not available (but rarely needed)

---

## Interview talking point

*"I chose PostgreSQL over SQL Server for its JSONB support, which I use for the audit log. Every state change stores a before/after snapshot in a JSONB column. This is queryable — I can write `WHERE old_values->>'status' = 'open'` — without needing 15 separate audit tables."*
