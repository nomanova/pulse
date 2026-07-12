# Contributor Quickstart Guide

## General

- The project uses .NET 10 and C# 14.0.
- The project is open source under the AGPL license.
- Entity framework is used as the ORM, with support for PostgreSQL and SQLite.

## Repository Layout

This repository is arranged using the clean architecture pattern.

- `src/presentation`: hosts the ASP.NET API endpoints and request response models
- `src/application`: hosts the command, query, and event handlers, as well as the DTO models
- `src/domain`: hosts the domain entities, using a DDD approach
- `src/infrastructure`: hosts the database and other infrastructure components

## Dependency Policy

- Before adding a new NuGet package, verify that its license is compatible with AGPL.
- Do not add packages that are commercial, source-available, trialware, or dual-licensed under a commercial license.
- Examples of prohibited libraries include MassTransit, FluentAssertions, AutoMapper, and MediatR.
- Avoid introducing dependencies when a small internal implementation is enough.
- Document the reason for any new dependency in the pull request or commit message.

## Coding Style

- Use idiomatic C# 14.0 and .NET 10 APIs.
- Prefer clear domain names over abbreviations.
- Keep methods small and focused.
- Prefer immutable types where practical.
- Use nullable reference types correctly.
- Do not introduce broad refactors while making targeted changes.
- Avoid unrelated formatting changes.

## Architecture Rules

- Follow the Clean Architecture dependency direction:
    - `domain` must not depend on application, infrastructure, or presentation.
    - `application` may depend on domain.
    - `infrastructure` may depend on application and domain.
    - `presentation` may depend on application.
- Keep domain logic in the domain layer.
- Keep persistence, external services, and framework-specific implementations in infrastructure.
- Do not bypass the application layer from presentation when handling use cases.

## CQRS Guidance

- The CQRS pattern is used to decouple the presentation layer from the application layer.
- Use commands for state-changing operations.
- Use queries for read-only operations.
- Keep command/query handlers focused on a single use case.
- Do not put business rules directly in handlers if they belong to domain entities or domain services.
- DTOs should live in the application layer unless they are transport-specific request/response models.

## API Guidance

- Keep request/response models in the presentation layer.
- Validate transport-level concerns at the API boundary.
- Delegate use cases to the application layer.
- Do not expose domain entities directly as API responses.
- Keep the endpoint / controller logic thin.

## Security Guidance

- Never log passwords, tokens, security stamps, or personally sensitive data.
- Do not store plaintext secrets in source control.
- Use abstractions for time, identity, hashing, and external services to keep code testable.
- Treat authentication and authorization changes as security-sensitive and add tests.

## Entity Framework Guidance

- Keep EF Core configuration in the infrastructure layer.
- Do not expose EF entities directly from API endpoints.
- Prefer domain entities and value objects over persistence-specific models unless there is a clear reason not to.
- Avoid lazy-loading assumptions unless explicitly configured.
- Migrations should be reviewed carefully and should not include unrelated schema changes.

## Test Conventions

- Each project has its own test project with a `.Tests` suffix.
- The internals of each project are exposed to its test project.
- Tests should be written using `xUnit` and `Moq`.
- The `src/domain/Pulse.Tests.Shared` project is referenced by all test projects and can be used for shared test
  classes.
- Test names should follow the pattern: `MethodOrUseCase_WithCondition_ShouldExpectedResult`
- Prefer and explicitly comment the Arrange/Act/Assert structure.
- Unit tests should avoid real infrastructure dependencies.
- Use test builders and shared fakes from `Pulse.Tests.Shared` where appropriate.
- Add regression tests for bug fixes.
- Tests should be deterministic and not depend on current time, random ordering, external services, or local machine
  state.

## Useful Commands

- Restore packages:

  ```bash
  dotnet restore
  ```

- Build the solution:

  ```bash
  dotnet build --no-restore
  ```

- Run all tests:

  ```bash
  dotnet test --no-build
  ```

- Run tests for a specific project:

  ```bash
  dotnet test path/to/Project.Tests.csproj
  ```

## Definition Of Done

- All tests pass
- New code includes tests
- No build warnings