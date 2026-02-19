# ADR 005 — Use JWT with refresh token rotation

**Status:** Accepted
**Date:** 2025

---

## Context

We need an authentication strategy for a stateless REST API consumed by an Angular SPA.

Options considered:
- **Session-based auth (cookies + server session):** Simple, works well for server-rendered apps. Not ideal for SPAs + APIs — requires sticky sessions or shared session storage in multi-instance deployments.
- **JWT only (no refresh token):** Stateless, simple. But if you set a long expiry (days/weeks), a stolen token can't be revoked. If you set short expiry (minutes), users are logged out constantly.
- **JWT + Refresh Token Rotation (chosen):** Short-lived access token (15 minutes) + long-lived refresh token (7 days). Best of both worlds.

---

## Decision

Use **JWT access tokens** (15 minute expiry) + **refresh token rotation**:

1. Login returns: `access_token` (JWT, 15 min) + `refresh_token` (opaque, 7 days)
2. Angular stores `access_token` in memory (not localStorage — XSS safe)
3. Angular stores `refresh_token` in HttpOnly cookie (not accessible to JavaScript — XSS safe)
4. When access token expires, Angular silently calls `POST /auth/refresh`
5. Server issues a **new** access token AND a **new** refresh token, invalidating the old one
6. If a refresh token is used twice (stolen + reused), the server detects it and invalidates the entire family — logging out the user everywhere

The refresh token is stored **hashed** in the database, never in plain text.

---

## Consequences

**Good:**
- Short access token window (15 min) limits the damage of a stolen token
- Refresh token rotation means a stolen refresh token is detected if the attacker tries to use it after the legitimate user already rotated it
- Stateless access tokens — no database lookup on every request
- Standard pattern used by major auth providers (Auth0, Okta, Firebase)

**Tradeoffs:**
- More complex to implement than simple long-lived JWT
- Requires careful client-side management of token refresh timing
- If refresh token storage (HttpOnly cookie) is compromised, attacker has 7 days

---

## Interview talking point

*"I implemented refresh token rotation where every refresh call invalidates the old token and issues a new one. This means if a refresh token is stolen and used, the next time the legitimate user's client tries to refresh, the server detects the reuse and invalidates the entire token family — both the attacker and the user are logged out. The user re-authenticates; the attacker loses access."*
