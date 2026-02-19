# DevOps Guide

This folder documents all CI/CD and infrastructure decisions for TaskFlow Pro.

---

## What is DevOps? (for this project)

DevOps is the set of practices that automate getting code from your laptop to production reliably.

In this project that means:

```
You write code
    → push to GitHub
        → GitHub Actions runs tests automatically (CI)
            → if tests pass, build Docker images (CI)
                → push images to registry (CD)
                    → deploy to server (CD - future)
```

---

## Tools Used

| Tool | What it does | When we use it |
|---|---|---|
| **Docker** | Packages the app into a container | Phase 8 |
| **Docker Compose** | Runs multiple containers together | From Phase 1 |
| **GitHub Actions** | Runs CI/CD pipelines automatically | Phase 8 |
| **.env files** | Manages config per environment | Phase 1 |
| **MinIO** | S3-compatible storage, runs in Docker | Phase 6 |

---

## Environments

| Environment | Purpose | Infrastructure |
|---|---|---|
| `local` | Developer's machine | Docker Compose |
| `ci` | GitHub Actions runners | Docker in Actions |
| `production` | Future — real users | TBD (Railway / Render / VPS) |

---

## Files

| File | Description |
|---|---|
| [docker.md](docker.md) | Docker concepts, Dockerfile explained |
| [docker-compose.md](docker-compose.md) | docker-compose.yml walkthrough |
| [github-actions.md](github-actions.md) | CI/CD pipeline explained |
| [environments.md](environments.md) | How environment variables work |

---

## Quick Reference

```bash
# Start everything locally
docker-compose up -d

# Check what's running
docker ps

# See logs for the API container
docker-compose logs -f api

# Stop everything (keep data)
docker-compose down

# Stop everything (DELETE data too)
docker-compose down -v

# Rebuild images after code changes
docker-compose up -d --build
```
