# ADR 004 — Use NgRx for Angular state management

**Status:** Accepted
**Date:** 2025

---

## Context

The board feature requires complex shared state: multiple components display the same tasks, drag & drop must update state instantly (optimistic), and SignalR events from other users must update the UI automatically.

Options considered:
- **Component-level state (local):** Simple, but can't share state between components without prop drilling.
- **Angular Services with BehaviorSubject:** Good for simpler apps. Less boilerplate, but harder to debug and no built-in devtools.
- **NgRx (chosen):** More boilerplate, but provides unidirectional data flow, Redux DevTools integration, and a clear pattern for optimistic updates + rollback.

---

## Decision

Use **NgRx** (Store, Effects, Selectors) for global state.

Applied to:
- Authentication state (current user, token)
- Board state (columns, tasks, drag & drop optimistic updates)
- Notifications state (unread count, notification list)

Simple feature state (form data, UI toggles) stays local in components.

---

## Consequences

**Good:**
- Unidirectional data flow — easy to trace where state changes come from
- Redux DevTools: see every action dispatched and state snapshot — invaluable for debugging
- Optimistic updates with rollback are a natural fit for NgRx Effects
- Effects handle all side effects (API calls, SignalR) in one place
- Selectors are memoized — performance is automatic

**Tradeoffs:**
- Significant boilerplate: actions, reducer, effects, selectors for each feature
- Learning curve for developers new to reactive programming
- Overkill for simple CRUD pages — only justified for complex shared state

---

## Interview talking point

*"I used NgRx for the board state because it naturally supports the optimistic update pattern. When a user moves a task, the reducer updates state immediately for instant feedback. The effect handles the API call in the background. If the API returns an error, a failure action rolls the state back to the previous position. This all happens without any component knowing whether the operation succeeded or failed yet."*
