# Implementation Plan for Backend
Frontend plan not set at the moment.

1. Solidify the portfolio domain
   - Introduce a dedicated `Portfolio` aggregate (e.g., broker, bank) that owns currency-specific `Account` children, where the currency is for visual displaying and as well the default curreny for that `Account`. But keep in mind that an `Account` should also can have transactions with a different currency, with an FX where the user later in the frontend can either provide or it will take the FX of that Day from History saved i.e. for USD-EUR.
   - Revisit `Account`, `Activity`, and `Asset` models so they support cash-only flows (deposits/withdrawals/transfers) without requiring an `AssetId`.
   - Fix enum naming (e.g., `Cryptocurrency`) and align precision constants with desired financial accuracy.
   - Decide on audit metadata (created/updated timestamps, soft deletes) and propagate consistently across entities.

## Step 1 Plan: Solidify the portfolio domain

- **Goals**
   - Model ownership boundaries so that every `Portfolio`, `Account`, and `Activity` implicitly associates with the placeholder user (`Guid.Empty`) until user management is available.
   - Ensure cash-only and multi-currency activity flows are expressible without bloating the aggregate.
   - Align monetary precision and enum naming to avoid churn in later migration steps.

- **Workstreams**
   - **Portfolio aggregate**
      - Add `backend/Zuricos.Folio.Data/Models/Portfolio.cs` with fields: `Id`, `Name`, `Institution`, `DisplayCurrency`, `Notes`, `UserId` (`Guid.Empty` default), `CreatedUtc`, `UpdatedUtc`, and soft-delete metadata.
      - Extend `backend/Zuricos.Folio.Data/Models/Account.cs` to include `PortfolioId`, `DisplayCurrency`, optional `Category`, and audit metadata consistent with `Portfolio`.
      - Document aggregate invariants (e.g., `DisplayCurrency` must be ISO 4217 uppercase, `Portfolio` cannot be soft-deleted while accounts remain active).
   - **Activity model flexibility**
      - Make `AssetId` nullable in `Activity`, introduce `ActivityKind` values for cash actions, and add FX fields (`BookCurrency`, `CounterCurrency`, `FxRate`, `SettledAmount`) to support cross-currency transactions.
      - Normalize enum naming under `backend/Zuricos.Folio.Data/Const` (e.g., rename `CryptoCurrency` to `Cryptocurrency`, ensure casing alignment) and update references.
   - **Common value objects & precision**
      - Confirm precision constants in `ConstValues` meet financial requirements (e.g., 18,8 for FX rates, 19,4 for amounts) and add new constants if needed.
      - Evaluate introducing lightweight structs for `Money` and `CurrencyCode`; capture decision and rationale (implement now or defer) in project notes.
   - **Audit and soft delete policy**
      - Decide between a shared auditable interface vs. `OwnedEntity` configuration; implement consistent mapping for `CreatedUtc`, `UpdatedUtc`, and `IsDeleted` across `Portfolio`, `Account`, `Activity`, `Asset`, `AssetHistory`.
      - Plan lifecycle hooks (e.g., soft-delete cascade for accounts when a portfolio is archived) without yet implementing persistence changes.

- **Deliverables**
   - Updated domain model classes and supporting constants reflecting the refined aggregate structure.
   - Draft of EF Core configuration changes (to be finalized in Step 2) noted in `backend/Zuricos.Folio.Data/FolioDbContext` comments or TODOs.
   - Documentation updates: invariants recorded in `documentation/` or inline XML summaries, plus guidance on the `Guid.Empty` user convention in `README.md` or developer notes.

- **Open questions / dependencies**
   - Confirm whether `Portfolio` should support multiple base currencies or a single display currency with per-account overrides.
   - Decide how transfers between accounts within the same portfolio should be represented (single activity vs. paired activities) before modeling invariants.
   - Identify any migration blockers (e.g., existing data) that need alignment once Step 2 begins.

2. Refine persistence layer and migrations
   - Update `FolioDbContext` configurations to reflect the finalized domain (relationships, cascade rules, indexes, constraints).
   - Add or adjust EF Core migrations (including seeding reference data such as default activity types if needed).
   - Validate PostgreSQL compatibility for all precision settings and ensure migrations compile under `Zuricos.Folio.Migrations.Psql`.

3. Establish application services
   - Create interfaces and implementations for core workflows: portfolio management, account management, transaction orchestration, asset catalog maintenance.
   - Leverage `IDbContextFactory<FolioDbContext>` for scoped operations and encapsulate transactional logic where consistency is required.
   - Introduce DTO mappers (manual or with a lightweight mapper) to isolate EF entities from API payloads.

4. Build API surface
   - Add minimal API endpoints which uses services so the api is just the description without logic, for portfolios, accounts, activities, and assets following RESTful conventions.
   - Implement CRUD plus scenario-specific endpoints (e.g., transaction posting, manual balance adjustments).
   - Apply input validation (FluentValidation or model attributes) and return `Result<T>` responses with `TypedResults` aligned with existing conventions.

5. Implement finance provider integrations
   - Define an abstraction (e.g., `IAssetPriceProvider`) and concrete adapters for CoinGecko and Finance.NET packages, keep in mind that CoinGecko is for cryptocurrency and Finance.NET for stock market etc, use Yahoofinance there if possible.
   - Handle rate limiting, error translation, and caching/fallback logic; support historical price fetches for charting.
   - Store fetched data in `AssetHistory`, ensuring deduplication and incremental updates.

6. Portfolio valuation and reporting
   - Compute holdings positions from activities (supporting cash and asset quantities).
   - Provide valuation endpoints combining latest prices, currency conversion, and aggregated performance metrics per portfolio/account.
   - Plan for currency conversion service (manual rates initially, extensible to live FX providers).

7. Validation, error handling, and observability
   - Standardize domain and application exceptions, mapping them to ProblemDetails responses.
   - Introduce structured logging scopes for portfolio/account identifiers and enrich telemetry for provider integrations.
   - Add global filters/middleware for correlation IDs and request logging as needed.

8. Seed data and tooling
   - Provide sample data scripts or command-line utilities for local development (portfolios, accounts, assets, activities).
   - Document setup instructions in `README` (database provisioning, environment variables, running migrations).
   - Automate migration execution via Docker Compose services in `ci-cd/` if applicable.

9. Testing strategy
   - Create xUnit projects under `tests/` for domain, application services, and API contract tests. 
   - Add integration tests using `WebApplicationFactory` to cover critical flows (portfolio creation, activity posting, valuation).
   - Mock provider interfaces to validate edge cases (missing prices, rate limits, partial history refresh).

10. Prepare frontend scaffolding
   - Initialize the React + Tailwind CSS 4 + DaisyUI stack under `frontend/` with a minimal landing page.
   - Define API client services and shared models aligning with backend DTOs.
   - Plan UI flows for portfolio listing, account drill-down, and transaction entry; stage mock data until APIs stabilize.
