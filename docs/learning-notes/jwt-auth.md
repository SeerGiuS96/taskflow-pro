# JWT Authentication — My Notes

**Phase:** 1
**Status:** [ ] To write

---

## What is a JWT? (in my own words)

> Explain the 3 parts: header, payload, signature.
> What makes it "self-contained"? Why doesn't the server need to look it up in the database?

---

## The authentication flow step by step

> Draw or describe what happens from "user clicks login" to "user accesses a protected endpoint":

1. User sends email + password to POST /auth/login
2. Server...
3. ...
4. Client receives...
5. On the next request...

---

## What is the refresh token and why do we need it?

> If JWT is stateless and secure, why do we also need a refresh token?
> What problem does a 15-minute expiry solve vs 7-day expiry?

---

## Refresh token rotation — how does it work?

> Explain the rotation strategy. What happens if a token is stolen?

---

## Where does the Angular app store the tokens?

> Where does the access token live? Where does the refresh token live?
> Why NOT localStorage for the refresh token?

---

## What was confusing at first?

---

## How I would explain it in an interview

---

## Code example from this project

```csharp
// Paste JwtTokenService.cs key parts here
```
