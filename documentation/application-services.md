# Application Services Catalogue

## Overview
This document enumerates the core application workflows derived from the completed domain modeling (Plan Step 1). Each workflow will be formalized behind an application service interface under `Zuricos.Folio.Api/Application`. The goal is to keep controllers thin, centralize transactional boundaries, and enforce domain invariants consistently.

All services return either `Result<T>` or `Result` (non-generic) to capture success, validation issues, and transient errors. API layers will convert them into ASP.NET Core `TypedResults`.

## Portfolio Management (`IPortfolioService`)
- **CreatePortfolioAsync**: Accepts `PortfolioCreateCommand`; validates ISO currency, uniqueness per user, and optional metadata. Persists portfolio with default timestamps.
- **UpdatePortfolioAsync**: Accepts `Guid portfolioId` and `PortfolioUpdateCommand`; enforces immutable invariants (e.g., cannot change user), updates name, display currency, and category.
- **ArchivePortfolioAsync**: Soft-deletes portfolio when no active accounts remain; returns validation errors when constraints fail.
- **GetPortfolioAsync**: Retrieves portfolio summary with aggregated account counts and display currency.
- **ListPortfoliosAsync**: Paginates portfolios for the current user with optional filtering by category or name.

## Account Management (`IAccountService`)
- **CreateAccountAsync**: Accepts `AccountCreateCommand`; ensures portfolio ownership, valid display currency, and default timestamps.
- **UpdateAccountAsync**: Accepts `AccountUpdateCommand`; updates mutable fields (name, category, type, display currency) while respecting soft-delete state.
- **ArchiveAccountAsync**: Soft-deletes the account; rejects if pending transfer pairs exist or account already deleted.
- **GetAccountAsync**: Returns account details plus latest cash/asset balances (computed via `ActivityService`).
- **ListAccountsByPortfolioAsync**: Lists accounts scoped to a single portfolio with simple filters (type, category).

## Activity Orchestration (`IActivityService`)
- **PostCashActivityAsync**: Accepts `CashActivityCreateCommand`; handles deposits, withdrawals, and fees while validating FX inputs.
- **PostAssetTradeAsync**: Accepts `AssetTradeCreateCommand`; handles buy/sell with asset linkage, cost basis tracking, and fee recording.
- **PostTransferAsync**: Accepts `TransferCreateCommand`; creates paired activities across accounts, ensuring FX coherence and preventing circular references.
- **ListActivitiesAsync**: Returns activities filtered by account, asset, type, and date range; supports pagination.
- **GetActivityAsync**: Fetches a single activity with contextual account + asset metadata.

## Asset Catalog (`IAssetCatalogService`)
- **CreateAssetAsync**: Accepts `AssetCreateCommand`; registers new asset instrument, enforcing symbol uniqueness and ISIN validation.
- **UpdateAssetAsync**: Accepts `AssetUpdateCommand`; allows currency, name, sector, and metadata updates while blocking symbol changes unless flagged for migration.
- **GetAssetAsync**: Retrieves asset details with latest price snapshot when available.
- **ListAssetsAsync**: Indexes assets with search by symbol/name/class.
- **EnsureSeedAssetsAsync**: Optional bootstrap routine to guarantee baseline assets exist (e.g., cash placeholders).

## Cross-Cutting Concerns
- **Validation**: Each service will return structured validation errors; prefer dedicated models (e.g., `ValidationError`) to avoid magic strings.
- **Transactions**: Multi-entity operations (portfolio + accounts, transfer pairs) will use EF Core transactions via `IDbContextFactory<FolioDbContext>` to maintain consistency.
- **Mapping**: DTOs will live under `Zuricos.Folio.Api/Application/Contracts`. Mapping helpers (static converters) will translate between DTOs and EF entities without leaking EF-specific constructs.
- **Auditing**: Services are responsible for maintaining `CreatedUtc`, `UpdatedUtc`, `IsDeleted`, and `DeletedUtc` fields according to domain conventions.

## Pending Questions
- Do we expose soft-deleted entities via admin endpoints? (current assumption: no)
- Should `ActivityService` compute derived metrics (running balances) or delegate to a reporting service? (current assumption: compute minimal ledger context only)
- What shape should error payloads take across services to align with future ProblemDetails integration?
