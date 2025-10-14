# Repository Custom Instructions for GitHub Copilot

## Project Overview
- Personal finance portfolio platform owned by Zuricos.
- Backend built with ASP.NET Core 9 Web API (`backend/Zuricos.Folio.Api`).
- Domain layer `Zuricos.Folio.Data` exposes EF Core models and the `FolioDbContext`.
- PostgreSQL is the primary data store, with EF Core migrations in `Zuricos.Folio.Migrations.Psql`.
- Frontend will be implemented with React, Tailwind CSS 4, and DaisyUI; the `frontend/` folder is a placeholder until scaffolding begins.
- MVP scope: unauthenticated users can create portfolio containers (e.g., brokers, banks) with currency-specific sub-accounts and record manual transactions (deposit, withdrawal, buy, sell, dividend, fee, tax). OIDC via Keycloak will follow later.

## Key Architectural Notes
- `Program.cs` composes the app via `SetupConfig` and `SetupServices` extension methods. Keep new bootstrapping logic inside these extensions instead of expanding `Program.cs`.
- DI container is configured through `builder.Services` in `SetupServices.cs`; register new services there.
- Database access is done via `IDbContextFactory<FolioDbContext>`; prefer injection of `FolioDbContext` or its factory instead of creating manual connections.
- Ensure migrations target the `Zuricos.Folio.Migrations.Psql` assembly and use Npgsql-compatible SQL.
- Treat the API as single-tenant without authentication until the dedicated Keycloak/OIDC workstream starts.

## Domain Model Highlights
- `Account`, `Asset`, `Activity`, `AssetHistory`, and `User` live in `backend/Zuricos.Folio.Data/Models` with fluent configuration in `FolioDbContext`.
- Monetary values use EF Core `HasColumnType` constants from `Zuricos.Folio.Data.Const.ConstValues`; continue that convention for precision-sensitive fields.
- `Activity` links to both `Account` and `Asset` using cascade deletes; do not change relationships without considering data integrity.

## Coding Conventions
- Target framework is `net9.0`; use modern C# 13 features where appropriate but avoid preview APIs unless already used.
- Stick to nullable reference types (project has `<Nullable>enable</Nullable>`).
- Return `IResult` or strongly typed `ActionResult<T>` in controllers; avoid magic strings for route names.
- Keep comments concise; only explain non-obvious logic.
- Prefer file-scoped namespaces and top-level statements, mirroring existing style.

## External Services & Config
- `appsettings*.json` stores environment configuration. Do not generate secrets; use placeholders and reference `local.env` for examples.
- Default connection string name is `psql`; configuration key `DatabaseProvider` chooses the provider (currently only `psql`).
- When adding new integrations (e.g., CoinGecko or Finance.NET), encapsulate them behind services with interfaces for easier testing.

## Testing & Tooling
- Future unit tests should use xUnit and live under a `tests/` root (not yet present). Suggest creating dedicated test projects rather than mixing tests into existing projects.
- Run `dotnet format` before committing significant C# changes.
- Containerised workflows live in `ci-cd/`; align any new Docker or CI steps with existing YAML conventions.

## Response Preferences for Copilot
- Provide incremental edits instead of full rewrites when modifying files.
- Highlight impacts on dependency injection, configuration, and EF Core modeling when proposing changes.
- When asked for examples, use the repository namespaces (`Zuricos.Folio.*`).
- Avoid introducing frameworks or patterns not already present without explicit user request (e.g., do not add MediatR or Dapper by default).
