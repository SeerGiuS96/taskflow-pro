# Frontend Architecture

## Angular 19 with Standalone Components

This project uses Angular 19 with **standalone components** (no NgModules for features). All state is managed with NgRx.

---

## Application Structure

```mermaid
graph TD
    App["AppComponent<br/>(root)"]

    App --> Core["Core<br/>(singleton services, loaded once)"]
    App --> Shared["Shared<br/>(reusable components)"]
    App --> Features["Feature Modules<br/>(lazy loaded)"]

    Core --> AuthSvc["AuthService"]
    Core --> Interceptors["HTTP Interceptors<br/>auth · error · tenant"]
    Core --> SignalRSvc["SignalRService<br/>(real-time connection)"]

    Features --> Auth["auth/<br/>(login, register)"]
    Features --> Dashboard["dashboard/<br/>(overview)"]
    Features --> Projects["projects/<br/>(list, detail)"]
    Features --> Board["board/<br/>(kanban, drag&drop)"]
    Features --> Admin["admin/<br/>(audit log, users)"]
```

---

## NgRx Data Flow

NgRx enforces a **unidirectional data flow**. This makes the app predictable and debuggable:

```mermaid
graph LR
    Component["Component<br/>(dispatches actions)"]
    Store["Store<br/>(holds state)"]
    Selector["Selector<br/>(reads state)"]
    Reducer["Reducer<br/>(pure function,<br/>computes new state)"]
    Effect["Effect<br/>(handles side effects:<br/>API calls, SignalR)"]
    API["API / SignalR"]

    Component -->|"dispatch(MoveTaskAction)"| Store
    Store --> Reducer
    Reducer -->|"new state"| Store
    Store --> Selector
    Selector -->|"async pipe"| Component
    Store --> Effect
    Effect -->|"HTTP call"| API
    API -->|"response"| Effect
    Effect -->|"dispatch success/failure action"| Store
```

### Optimistic Updates (the impressive pattern)

When a user drags a task to a new column:

```mermaid
sequenceDiagram
    participant U as User drags task
    participant C as BoardComponent
    participant S as NgRx Store
    participant E as BoardEffects
    participant API as .NET API

    U->>C: Drop task on new column
    C->>S: dispatch(moveTask({taskId, newColumnId}))
    S->>S: Reducer updates state immediately (optimistic)
    C->>C: UI shows task in new column ← instant feedback
    S->>E: Effect picks up moveTask action
    E->>API: PATCH /tasks/123/move
    alt Success
        API-->>E: 200 OK
        E->>S: dispatch(moveTaskSuccess)
    else Failure
        API-->>E: 400 / 500 Error
        E->>S: dispatch(moveTaskFailure({previousColumnId}))
        S->>S: Reducer rolls task back to original column
        C->>C: UI shows task back where it was
    end
```

---

## HTTP Interceptors

Interceptors run automatically on every HTTP request:

```mermaid
graph LR
    Request["HTTP Request"] --> Auth
    Auth["AuthInterceptor<br/>adds Authorization: Bearer token"] --> Tenant
    Tenant["TenantInterceptor<br/>adds X-Organization: slug header"] --> Error
    Error["ErrorInterceptor<br/>handles 401, 403, 500 globally"] --> API["API"]
    API --> Error
    Error --> Component["Component receives clean response"]
```

---

## Route Guards

```mermaid
graph TD
    Route["Navigate to /board/123"] --> AuthGuard
    AuthGuard{"AuthGuard<br/>is user logged in?"} -->|No| Login["Redirect to /login"]
    AuthGuard -->|Yes| RoleGuard
    RoleGuard{"RoleGuard<br/>has required role?"}  -->|No| Forbidden["Show 403 page"]
    RoleGuard -->|Yes| Component["Load BoardComponent"]
```

---

## Folder Structure

```
frontend/
└── src/
    └── app/
        ├── core/                         # Singleton - imported once in app.config.ts
        │   ├── auth/
        │   │   ├── auth.service.ts       # login, logout, token management
        │   │   ├── auth.guard.ts         # protects routes requiring login
        │   │   └── role.guard.ts         # protects routes requiring a role
        │   ├── interceptors/
        │   │   ├── auth.interceptor.ts   # attaches Bearer token to every request
        │   │   ├── tenant.interceptor.ts # attaches org header to every request
        │   │   └── error.interceptor.ts  # global error handling (401 → logout, etc.)
        │   └── services/
        │       └── signalr.service.ts    # manages SignalR connection lifecycle
        │
        ├── shared/                       # Reusable across features
        │   ├── components/
        │   │   ├── avatar/               # User avatar with initials fallback
        │   │   ├── confirm-dialog/       # "Are you sure?" modal
        │   │   ├── empty-state/          # Illustration + message when no data
        │   │   ├── loading-spinner/
        │   │   └── priority-icon/        # Colored icon for task priority
        │   ├── directives/
        │   │   └── has-role.directive.ts # *hasRole="'admin'" — hides element if no role
        │   └── pipes/
        │       ├── time-ago.pipe.ts      # "3 minutes ago"
        │       └── file-size.pipe.ts     # "2.4 MB"
        │
        ├── features/
        │   ├── auth/                     # Lazy loaded
        │   │   ├── login/
        │   │   │   ├── login.component.ts
        │   │   │   └── login.component.html
        │   │   ├── register/
        │   │   └── auth.routes.ts
        │   │
        │   ├── board/                    # Most complex feature
        │   │   ├── board.component.ts    # Main kanban board
        │   │   ├── board-column/         # A single column
        │   │   ├── task-card/            # A task card (draggable)
        │   │   ├── task-detail/          # Slide-over panel with full task info
        │   │   │   ├── task-comments/
        │   │   │   └── task-attachments/
        │   │   └── board.routes.ts
        │   │
        │   └── admin/                    # Role-gated feature
        │       ├── audit-log/
        │       └── admin.routes.ts
        │
        └── store/                        # NgRx state management
            ├── app.state.ts              # Root state interface
            ├── auth/
            │   ├── auth.actions.ts       # login, loginSuccess, loginFailure, logout
            │   ├── auth.effects.ts       # calls API, dispatches success/failure
            │   ├── auth.reducer.ts       # pure function: state + action = new state
            │   └── auth.selectors.ts     # selectCurrentUser, selectIsAuthenticated
            ├── board/
            │   ├── board.actions.ts      # loadBoard, moveTask, moveTaskSuccess...
            │   ├── board.effects.ts      # API calls, SignalR event dispatch
            │   ├── board.reducer.ts      # handles optimistic updates + rollback
            │   └── board.selectors.ts    # selectColumns, selectTasksByColumn
            └── notifications/
                └── ...
```

---

## Component Communication Pattern

```mermaid
graph TD
    Parent["Parent Component"] -->|"@Input()"| Child["Child Component"]
    Child -->|"@Output() EventEmitter"| Parent
    Child -->|"dispatch action"| Store["NgRx Store"]
    Store -->|"selector | async"| Parent
    Store -->|"selector | async"| Child
```

**Rule:** Components do not call services directly for data. They dispatch actions and select from the store. Services are only called from NgRx Effects.
