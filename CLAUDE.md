# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Pulse is a self-hosted, open-core (AGPLv3) notification server for .NET: workflow engine, multichannel delivery (email, SMS, ...), and a provider plugin system. Early development. .NET 10 / C# 14 (SDK pinned in `global.json`), EF Core with PostgreSQL or SQLite. Solution: `Pulse.sln`. The backlog is managed in Jira; `PLS-<n>` refers to the Jira ticket number.

## Commands

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet test path/to/Project.Tests.csproj                          # one test project
dotnet test path/to/Project.Tests.csproj --filter "FullyQualifiedName~SomeTest"   # single test

# Run the full stack (API + proxy, optional Web) through Aspire
cd src/Pulse.AppHost && ./run.sh
```

AppHost is configured through parameters in `src/Pulse.AppHost/appsettings*.json`: `DatabaseProvider` (`Sqlite`|`Postgres`), `DatabaseOnly` (start only the DB), `WithWeb` (also start Blazor `Pulse.Web`), plus DB path/credentials. It sets `PULSE_ADMIN_PASSWORD` for the API. Postgres runs as a persistent container.

Docs site (`docs/`, VitePress): `npm run dev` / `npm run build` there. `http/` contains HTTP-client request files and env files for manual API calls.

CI (`.github/workflows`) publishes API (Docker image), CLI, docs and PDK on tags like `api/v1.2.3`, `cli/v1.2.3`.

## Architecture

Clean Architecture with CQRS, under `src/`:

- `domain/` — `Pulse.Domain` (DDD entities), `Pulse.Domain.Channels`, `Pulse.Tests.Shared` (builders/fakes referenced by all test projects).
- `application/` — `Pulse.App` (command/query/event handlers, `Handlers/`), DTO projects (`Pulse.App.Dto`, `Pulse.Application.Dto`).
- `infrastructure/` — `Pulse.Infra` (EF Core `Database/`, `Plugins/` loading, `Security/`, `Services/`).
- `presentation/` — `Pulse.Api` (host: `Program.cs`, `Setup.cs`, `Startup.cs`) plus the API surface split into **Ctrl** (control plane / management), **Data** (data plane, e.g. triggering workflows) and **Shared**. Each has `*.Contract` (request/response models) and `*.Client` (typed client) projects. `Pulse.Web` / `Pulse.Web.Core` are the Blazor management UI.
- `plugins/` — `Pulse.Plugin` (plugin abstractions/PDK) and providers (`Email.Console`, `Email.SendGrid`, `Sms.Console`).
- `tools/Pulse.Cli` — CLI over the API clients.
- `Pulse.Proxy` — YARP reverse proxy fronting API and Web; `Pulse.AppHost` — Aspire orchestration.

Each layer registers its services via a `Setup.cs`.

Dependency direction: domain depends on nothing; application → domain; infrastructure → application + domain; presentation → application. Presentation must go through application handlers (no bypass), keep endpoints thin, keep request/response models in presentation, and never expose domain or EF entities from the API. EF configuration and migrations live in infrastructure; migrations must contain no unrelated schema changes. Business rules belong in domain entities/services, not handlers.

## Conventions

- Dependencies must be AGPL-compatible. No commercial/source-available/dual-licensed packages (e.g. MassTransit, FluentAssertions, AutoMapper, MediatR are prohibited). Prefer a small internal implementation over a new package; justify any new dependency in the commit/PR message.
- Tests: xUnit + Moq, one `*.Tests` project per project (internals visible to it). Names `MethodOrUseCase_WithCondition_ShouldExpectedResult`, with explicit `// Arrange`, `// Act`, `// Assert` comments. Tests must be deterministic (no real time, randomness, external services). Add regression tests for bug fixes; auth changes need tests.
- Use abstractions for time, identity, hashing and external services. Never log passwords, tokens, security stamps or personal data.
- Keep changes targeted: no broad refactors or unrelated formatting. Prefer immutable types, correct nullable annotations, clear names.
- Done = all tests pass, new code has tests, no build warnings.
- Commit messages are prefixed with the Jira ticket key (e.g. `PLS-11 ...`).
