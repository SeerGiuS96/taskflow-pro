# ADR 006 — Use Docker Compose for local development

**Status:** Accepted
**Date:** 2025

---

## Context

The project requires PostgreSQL, Redis, and MinIO running locally. Options:
- Install each service natively on Windows
- Use Docker Compose

---

## Decision

Use **Docker Compose** to run all infrastructure services locally.

Developers only need Docker Desktop installed. A single `docker-compose up -d` starts everything.

---

## Consequences

**Good:**
- Zero native installations — no PostgreSQL installer, no Redis for Windows
- Identical environment for every developer and CI
- Easy to reset: `docker-compose down -v` removes everything, `up` starts fresh
- Matches production shape (containers)

**Tradeoffs:**
- Requires Docker Desktop (free for personal use)
- Slightly slower I/O than native on Windows (WSL2 mitigates this)
