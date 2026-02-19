# ADR 007 — Use Mermaid for architecture diagrams

**Status:** Accepted
**Date:** 2025

---

## Context

We need diagrams for architecture, ERD, UML, and sequence flows. Options:
- Mermaid (text-based, renders in GitHub)
- PlantUML (text-based, needs external rendering or plugin)
- Draw.io / Lucidchart (GUI tools, exports as image)

---

## Decision

Use **Mermaid** embedded in Markdown files.

Reasons:
- GitHub renders Mermaid natively in `.md` files — no plugins needed
- VS Code renders it with the "Markdown Preview Mermaid Support" extension (free)
- Diagrams live in the repo as text — version controlled, diffable
- Simple syntax, sufficient for this project's needs

---

## Consequences

**Good:**
- Diagrams are always in sync with the code (they're in the same repo)
- No external tools needed — works in GitHub, VS Code, GitLab
- Text format means you can see diagram changes in git diff

**Tradeoffs:**
- Less visual control than GUI tools
- Very complex diagrams can be hard to write in text form
- PlantUML would be better for large enterprise diagrams with hundreds of entities
