# Session 01 — Key Concepts

Questions I asked and what I understood from the answers.
Written to consolidate what I learned in this session.

---

## What is Multi-tenancy?

A single system serving multiple independent companies (tenants).
Each company sees only their own data, even though they share the same database and application.

Example: Slack, Jira, Notion — thousands of companies use the same software, but each lives in its own isolated "workspace."

In this project: every table has an `organization_id` column. EF Core adds a `WHERE organization_id = ?` filter automatically to every query. A developer can't forget to filter — the system does it for them.

**Why it matters:** Most enterprise SaaS products are multi-tenant. Understanding this pattern is a key differentiator.

---

## What is CQRS?

Separating operations into two types:

- **Command** = changes something (CreateTask, MoveTask, DeleteProject)
- **Query** = reads something (GetTaskById, SearchTasks)

Why separate them? Reads and writes have different needs. Reads need speed. Writes need validation and business logic. Keeping them separate makes the code cleaner and easier to scale.

**Without CQRS:** one big `TaskService` class with 20+ methods, hard to navigate and test.

**With CQRS:** each operation is its own small, focused class. Easy to find, easy to test in isolation.

---

## What is MediatR?

A .NET library that implements the Mediator pattern.

Instead of a controller calling a service directly, it sends a message. MediatR routes that message to the correct handler.

```csharp
// Without MediatR — controller knows too much
var task = _taskService.Create(title, projectId, userId, priority...);

// With MediatR — controller just sends a message
var task = await _mediator.Send(new CreateTaskCommand(title, projectId));
```

The handler receives the message and processes it. The controller doesn't know who resolves it or how.

---

## What is a Pipeline of Behaviors?

The most elegant feature of MediatR. Every request passes through a "tunnel" of behaviors before reaching the handler:

```
Request enters
    → ValidationBehavior     (validates inputs)
    → AuthorizationBehavior  (checks permissions)
    → LoggingBehavior        (logs the request)
    → PerformanceBehavior    (measures duration)
    → Handler                (does the actual work)
```

You write this logic ONCE. It applies automatically to every command and query. That's what "cross-cutting concerns" means — things that affect the whole system but aren't business logic.

---

## What is Clean Architecture?

Organizing code in concentric layers where dependencies only point inward.

```
API → Infrastructure → Application → Domain
                                      ↑
                              no external dependencies
```

The Domain layer (entities, business rules) has zero knowledge of Entity Framework, HTTP, or any external tool. This means:
- Business logic is testable without a database
- Changing the database only affects the Infrastructure layer
- The code is easier to understand — you always know where to put new code

---

## Mermaid vs PlantUML

Both are text-based diagramming tools. We chose Mermaid because:
- GitHub renders it natively in `.md` files — no plugins needed
- VS Code shows preview with a free extension
- Simple syntax, sufficient for this project

PlantUML is more powerful for very complex enterprise diagrams, but overkill here.

---

## Conventional Commits with Emoji

A commit message format that makes the Git history readable:

```
✨ feat(auth): add JWT refresh token rotation
🐛 fix(board): resolve task ordering off-by-one error
📝 docs(adr): add decision record for PostgreSQL
♻️ refactor(domain): extract Result pattern to base class
```

Format: `<emoji> <type>(<scope>): <short description>`

Why: a clean Git history is part of professional code. Interviewers and teammates look at commit history.

---

## Where does this chat live?

Claude Code (this terminal) is a separate product from Claude.ai (the desktop/web app).
Claude Code runs inside VS Code's integrated terminal and has direct access to project files.
Claude.ai is just a chat interface — it cannot read or edit files.

They don't sync. The context for this project lives in `CLAUDE.md`, which Claude Code reads at the start of every session.

---

## Encapsulation — private setters in entities

In `User.cs`, all properties have `private set`. This means nobody outside the class can do:

```csharp
user.Email = "hacked@evil.com"; // ❌ won't compile
```

The only way to change state is through explicit methods:

```csharp
user.SetRefreshToken(hash, expiresAt); // ✅ controlled change
user.RevokeRefreshToken();             // ✅ controlled change
```

**Why it matters:** the object controls its own data. This is called **encapsulation** — one of the four pillars of OOP. In domain entities it also means business rules live inside the entity, not scattered across the codebase.

---

## My questions to review before next session

- [ ] Can I explain what CQRS is in one sentence without looking at notes?
- [ ] Can I explain why we use 4 separate .NET projects instead of one?
- [ ] Can I explain what a pipeline behavior does and give one example?
- [ ] Can I explain what multi-tenancy is and how EF Core global filters work?
