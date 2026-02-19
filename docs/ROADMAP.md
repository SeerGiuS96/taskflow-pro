# Development Roadmap

Progress is tracked by **phase completion**, not by dates.
Advance to the next phase when you can explain what you built without looking at notes.

---

## Current Status

**Active Phase:** 0 — Foundation
**Progress:** Setting up structure and documentation

---

## Phase 0 — Foundation
> Goal: Understand the full system before writing a single line of application code.

- [x] Create project folder structure
- [x] Initialize Git with .gitignore
- [x] Write README.md
- [x] Write architecture documentation (backend, frontend, overview)
- [x] Write database ERD
- [x] Write Architecture Decision Records (ADRs)
- [x] Create learning notes templates
- [ ] Install Docker Desktop
- [ ] Create GitHub repository and push

**You are ready for Phase 1 when:**
You can draw the architecture on a whiteboard and explain what each layer does and why.

---

## Phase 1 — Backend Skeleton + Auth
> Goal: A working API with registration, login, and protected endpoints.

### Setup
- [ ] Create .NET solution with 4 projects (Domain, Application, Infrastructure, API)
- [ ] Install NuGet packages (MediatR, EF Core, FluentValidation, JWT)
- [ ] Configure EF Core with PostgreSQL connection
- [ ] Create `User` entity and first migration
- [ ] Set up Swagger with JWT support

### Auth
- [ ] `POST /auth/register` — hash password, create user
- [ ] `POST /auth/login` — validate, return JWT + refresh token
- [ ] `POST /auth/refresh` — refresh token rotation
- [ ] `POST /auth/logout` — revoke refresh token
- [ ] JWT middleware configured
- [ ] `GET /auth/me` protected endpoint (test it works)

### Learning notes to write
- [ ] `jwt-auth.md`
- [ ] `clean-architecture.md`
- [ ] `ef-core.md`

**You are ready for Phase 2 when:**
You can register, login, get a JWT, and call a protected endpoint. You can explain what JWT is and why refresh tokens exist.

---

## Phase 2 — CQRS Core Features
> Goal: Implement Organizations and Projects using CQRS pattern properly.

### Infrastructure
- [ ] Install and configure MediatR
- [ ] Create `ValidationBehavior`
- [ ] Create `LoggingBehavior`
- [ ] Create `Result<T>` pattern
- [ ] Create `BaseEntity` with Id, CreatedAt, UpdatedAt

### Organizations
- [ ] `Organization` entity + migration
- [ ] `OrganizationMember` entity (join table with role)
- [ ] `CreateOrganizationCommand` + Handler + Validator
- [ ] `GetOrganizationBySlugQuery` + Handler
- [ ] `InviteMemberCommand` + Handler

### Projects
- [ ] `Project` entity + migration
- [ ] `CreateProjectCommand` + Handler + Validator
- [ ] `GetProjectsQuery` + Handler
- [ ] Auto-create default Board + Columns on project creation

### Learning notes to write
- [ ] `cqrs-mediatr.md`

**You are ready for Phase 3 when:**
You can create an organization and projects via the API. You can explain the CQRS pattern and what a pipeline behavior does.

---

## Phase 3 — Multi-Tenancy + RBAC
> Goal: Every query is automatically scoped to the current organization. Users can only see their data.

- [ ] `TenantResolutionMiddleware` — reads org from route, sets tenant context
- [ ] `ITenantContext` interface and implementation
- [ ] EF Core global query filters on all tenant-scoped entities
- [ ] `AuthorizationBehavior` in MediatR pipeline
- [ ] Organization role enforcement (owner, admin, member, viewer)
- [ ] Project role enforcement

### Learning notes to write
- [ ] `multi-tenancy.md`

**You are ready for Phase 4 when:**
You can prove that User A from Org A cannot see Org B's data even if they guess the IDs. You can explain global query filters.

---

## Phase 4 — Angular Foundation + NgRx
> Goal: Angular app with authentication, routing, and NgRx set up.

- [ ] Create Angular 19 app (standalone)
- [ ] Configure Angular Material + Tailwind
- [ ] Set up NgRx store (auth slice)
- [ ] Login and Register pages with Reactive Forms
- [ ] Auth interceptor (attaches Bearer token)
- [ ] Error interceptor (handles 401 → logout)
- [ ] Auth guard (protects routes)
- [ ] Dashboard page (just a placeholder, but route-protected)

### Learning notes to write
- [ ] `ngrx.md`

**You are ready for Phase 5 when:**
You can log in via Angular, the JWT is attached to requests automatically, and protected routes redirect to login when not authenticated.

---

## Phase 5 — The Board Feature
> Goal: The most visually impressive feature. Drag & drop with real-time optimistic updates.

- [ ] Board component — displays columns and task cards
- [ ] Angular CDK drag-and-drop between columns
- [ ] `moveTask` NgRx action
- [ ] Optimistic update in reducer (move task before API responds)
- [ ] `moveTask` Effect (API call)
- [ ] Rollback in Effect on API error
- [ ] Task detail slide-over panel
- [ ] Markdown description editor
- [ ] Comments on tasks

**You are ready for Phase 6 when:**
Drag & drop works. If the API is offline, the task snaps back to its original position. You can explain optimistic updates and rollback.

---

## Phase 6 — Domain Events + Real-time
> Goal: Multiple users see live board updates without refreshing.

- [ ] `IDomainEvent` interface
- [ ] Domain events on `Task` entity (TaskCreatedEvent, TaskMovedEvent)
- [ ] `DomainEventInterceptor` in EF Core SaveChanges
- [ ] `AuditLog` entity and handler
- [ ] `Notification` entity and handler
- [ ] SignalR hub setup
- [ ] Angular SignalR service
- [ ] Board updates from SignalR dispatch NgRx actions

### Learning notes to write
- [ ] `domain-events.md`
- [ ] `signalr.md`

**You are ready for Phase 7 when:**
Open two browser windows. Move a task in one. It updates in the other automatically.

---

## Phase 7 — Testing
> Goal: Demonstrate senior-level testing habits.

- [ ] Unit tests for Domain entity methods
- [ ] Unit tests for all Command handlers (mocking repositories)
- [ ] Integration tests with Testcontainers (real PostgreSQL)
- [ ] Angular component tests
- [ ] NgRx reducer tests (pure function, easy)
- [ ] NgRx effect tests
- [ ] Target: 70%+ coverage on Application layer

### Learning notes to write
- [ ] `testing.md`

**You are ready for Phase 8 when:**
`dotnet test` and `ng test` both pass. You can explain the difference between unit and integration tests.

---

## Phase 8 — DevOps
> Goal: The whole stack runs with one command. Tests run on every push.

- [ ] Dockerfile for .NET API (multi-stage)
- [ ] Dockerfile for Angular (multi-stage, serve with Nginx)
- [ ] `docker-compose.yml` (API, Angular, PostgreSQL, Redis, MinIO)
- [ ] `docker-compose.dev.yml` overrides (hot reload, volume mounts)
- [ ] `.env.example` file
- [ ] GitHub Actions CI: on PR → run `dotnet test` + `ng test` + lint
- [ ] GitHub Actions CD: on merge to main → build + push Docker images
- [ ] Health check endpoints on API

### Learning notes to write
- [ ] `docker.md`
- [ ] `github-actions.md`

**You are ready for Phase 9 when:**
`docker-compose up` starts the full stack. A PR on GitHub triggers automatic tests.

---

## Phase 9 — Portfolio Polish
> Goal: Make it undeniably impressive to a hiring manager.

- [ ] Dashboard page with Chart.js (tasks per week, tasks by assignee)
- [ ] Task search with debouncing
- [ ] Admin panel with audit log viewer
- [ ] Dark mode toggle
- [ ] README with architecture diagram, screenshots, setup instructions
- [ ] Demo video (2-3 minutes walking through features)
- [ ] GitHub repository public, all ADRs complete
- [ ] Learning notes fully written

**You are done when:**
You can demo the app live in an interview and answer any question about every decision made.

---

## Interview Readiness Checklist

Before applying, verify you can answer these without hesitation:

- [ ] "Walk me through your project architecture"
- [ ] "What is CQRS and why did you use it?"
- [ ] "How does multi-tenancy work in your system?"
- [ ] "Explain your authentication strategy"
- [ ] "What are domain events and how did you use them?"
- [ ] "How do optimistic updates work in your Angular app?"
- [ ] "How do you test your application?"
- [ ] "How would you scale this system?"
- [ ] "What would you do differently if you started over?"
