# TaskFlow Pro

> A full-stack project management platform inspired by tools like Linear and Jira.
> Built with Clean Architecture, CQRS, Angular and real-time collaboration.

---

## What is this?

TaskFlow Pro is a **multi-tenant project management system** where organizations can manage projects, boards, and tasks with real-time collaboration.

TaskFlow Pro is a **multi-tenant project management system** where organizations manage projects, boards, and tasks with real-time collaboration.

Built to practice and apply:
- Clean Architecture and CQRS in .NET
- Angular with NgRx state management
- Domain-Driven Design concepts
- DevOps with Docker and GitHub Actions

---

## Tech Stack

### Backend
| Layer | Technology |
|---|---|
| Framework | .NET 9 Web API |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / API) |
| CQRS | MediatR |
| Validation | FluentValidation |
| ORM | Entity Framework Core 9 |
| Database | PostgreSQL 16 |
| Auth | JWT + Refresh Token Rotation |
| Real-time | SignalR |
| Cache | Redis |
| Storage | MinIO (local) / AWS S3 (prod) |
| Logging | Serilog |
| Testing | xUnit + Testcontainers |

### Frontend
| Layer | Technology |
|---|---|
| Framework | Angular 19 (standalone components) |
| State | NgRx 19 |
| UI | Angular Material + Tailwind CSS |
| Drag & Drop | Angular CDK |
| Real-time | @microsoft/signalr |
| Forms | Angular Reactive Forms |
| Testing | Jest + Angular Testing Library |

### Infrastructure
| Tool | Purpose |
|---|---|
| Docker + Docker Compose | Local development environment |
| GitHub Actions | CI/CD pipelines |
| MinIO | S3-compatible local object storage |

---

## Architecture Overview

```
┌─────────────────────────────────────────────────┐
│                   Angular 19                     │
│         NgRx · Material · CDK · SignalR          │
└─────────────────────┬───────────────────────────┘
                      │ HTTPS / WebSocket
┌─────────────────────▼───────────────────────────┐
│              .NET 9 Web API                      │
│  ┌──────────┐ ┌────────────┐ ┌───────────────┐  │
│  │  Domain  │ │Application │ │Infrastructure │  │
│  │ Entities │ │  CQRS /    │ │  EF Core /    │  │
│  │  Events  │ │  MediatR   │ │  SignalR /    │  │
│  │  Rules   │ │  FluentVal │ │  Storage      │  │
│  └──────────┘ └────────────┘ └───────────────┘  │
└──────────┬──────────────────────────────────────┘
           │
┌──────────▼──────────┐  ┌──────────┐  ┌─────────┐
│    PostgreSQL 16     │  │  Redis   │  │  MinIO  │
│  (primary database)  │  │ (cache / │  │ (files) │
│                      │  │ pubsub)  │  │         │
└──────────────────────┘  └──────────┘  └─────────┘
```

Full architecture details: [docs/architecture/](docs/architecture/)

---

## Key Features

- **Multi-tenancy** — Organizations are fully isolated at the database level
- **Kanban Board** — Drag & drop task cards with real-time sync
- **RBAC** — Role-based access control per organization and project
- **Real-time** — Live board updates via SignalR when teammates move tasks
- **Audit Log** — Every state change is recorded with before/after snapshots
- **Domain Events** — Decoupled side effects (notifications, audit) via event dispatch
- **Optimistic Updates** — UI responds instantly, rolls back on server error
- **File Attachments** — Upload files to tasks, stored in S3-compatible storage
- **Threaded Comments** — Nested comments with Markdown support

---

## Project Structure

```
taskflow-pro/
├── backend/                  # .NET Clean Architecture solution
│   ├── src/
│   │   ├── TaskFlowPro.Domain/
│   │   ├── TaskFlowPro.Application/
│   │   ├── TaskFlowPro.Infrastructure/
│   │   └── TaskFlowPro.API/
│   └── tests/
├── frontend/                 # Angular 19 application
│   └── src/
│       ├── app/
│       │   ├── core/
│       │   ├── shared/
│       │   ├── features/
│       │   └── store/
│       └── environments/
├── docs/                     # All project documentation
│   ├── architecture/         # System design diagrams
│   ├── database/             # ERD and schema decisions
│   ├── adr/                  # Architecture Decision Records
│   ├── uml/                  # UML diagrams
│   ├── devops/               # CI/CD and Docker guides
│   └── learning-notes/       # Personal notes written while building
├── .github/
│   └── workflows/            # GitHub Actions pipelines
├── docker-compose.yml        # Full stack local environment
├── docker-compose.dev.yml    # Development overrides
└── .env.example              # Environment variables template
```

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Angular CLI 19](https://angular.io/cli): `npm install -g @angular/cli`

### Quick Start

```bash
# 1. Clone the repository
git clone https://github.com/YOUR_USERNAME/taskflow-pro.git
cd taskflow-pro

# 2. Copy environment variables
cp .env.example .env

# 3. Start infrastructure (PostgreSQL, Redis, MinIO)
docker-compose up -d

# 4. Start backend
cd backend
dotnet run --project src/TaskFlowPro.API

# 5. Start frontend (new terminal)
cd frontend
npm install
ng serve
```

App available at: `http://localhost:4200`
API available at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

---

## Documentation

| Document | Description |
|---|---|
| [Architecture Overview](docs/architecture/overview.md) | System design and layer responsibilities |
| [Backend Architecture](docs/architecture/backend.md) | Clean Architecture layers, CQRS flow |
| [Frontend Architecture](docs/architecture/frontend.md) | Angular modules, NgRx data flow |
| [Database ERD](docs/database/erd.md) | Entity-Relationship diagram and schema |
| [ADR Index](docs/adr/README.md) | All Architecture Decision Records |
| [DevOps Guide](docs/devops/README.md) | Docker, CI/CD pipelines explained |
| [Learning Notes](docs/learning-notes/README.md) | Concepts explained in plain language |
| [Roadmap](docs/ROADMAP.md) | Development phases and progress |

---

## Commit Convention

This project follows [Conventional Commits](https://www.conventionalcommits.org/) with emojis:

| Emoji | Type | When to use |
|---|---|---|
| ✨ | `feat` | New feature |
| 🐛 | `fix` | Bug fix |
| ♻️ | `refactor` | Code change, no new feature or fix |
| 📝 | `docs` | Documentation only |
| ✅ | `test` | Adding or updating tests |
| 🔧 | `chore` | Build process, tooling, config |
| 🚀 | `ci` | CI/CD pipeline changes |
| 💄 | `style` | UI/styling changes |
| ⚡️ | `perf` | Performance improvement |
| 🗃️ | `db` | Database migrations or schema changes |

**Example:**
```
✨ feat(auth): add JWT refresh token rotation

- Implement sliding window refresh token strategy
- Store hashed token in database, not plain text
- Revoke old token on every refresh to prevent reuse
```

---

## License

MIT — built for learning and portfolio purposes.
