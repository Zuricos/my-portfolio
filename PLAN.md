# Implementation Plan for Backend
Frontend plan not set at the moment.

1. Solidify the portfolio domain ✅ **COMPLETED**
   - Introduce a dedicated `Portfolio` aggregate (e.g., broker, bank) that owns currency-specific `Account` children, where the currency is for visual displaying and as well the default curreny for that `Account`. But keep in mind that an `Account` should also can have transactions with a different currency, with an FX where the user later in the frontend can either provide or it will take the FX of that Day from History saved i.e. for USD-EUR.
   - Revisit `Account`, `Activity`, and `Asset` models so they support cash-only flows (deposits/withdrawals/transfers) without requiring an `AssetId`.
   - Fix enum naming (e.g., `Cryptocurrency`) and align precision constants with desired financial accuracy.
   - Decide on audit metadata (created/updated timestamps, soft deletes) and propagate consistently across entities.

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
