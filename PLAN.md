# Implementation Plan for Backend
Frontend plan not set at the moment.

> **Note for future contributors (incl. AI assistants):**
> After completing any implementation step below, update this plan immediately with a short summary of what changed so the roadmap stays accurate for the next agent.

1. Solidify the portfolio domain ✅ **COMPLETED**
   - Domain entities (`Portfolio`, `Account`, `Activity`, `Asset`) now model cash-only and mixed-currency flows with audit metadata and soft-delete flags.
   - Precision constants (money, quantity, FX) live in `ConstValues`, and enums were cleaned up for readability.
   - Portfolios own currency-aware account children, while activities store FX data so mismatched currencies can be reconciled later.

2. Refine persistence layer and migrations ✅ **COMPLETED**
   - `FolioDbContext` config matches the domain, including cascade rules, owned types, and indexes for lookups.
   - Latest EF Core migrations target the PostgreSQL project and compile under `Zuricos.Folio.Migrations.Psql`.
   - Column precision uses the shared constants, keeping Postgres schema consistent with money/quantity requirements.

3. Establish application services ✅ **COMPLETED**
   - Interfaces, DTOs, and mappers live under `Application/*`, keeping EF Core entities out of the transport layer.
   - `PortfolioService`, `AccountService`, `ActivityService`, and `AssetCatalogService` implement the full workflow catalog via `IDbContextFactory<FolioDbContext>` and guard clauses.
   - Result/validation patterns are standardized through `Result`/`ValidationError`, ready for controller translation.

4. Build API surface ✅ **COMPLETED**
   - Minimal API groups now expose portfolios, accounts, activities, and assets under `/api`, delegating directly to the existing services.
   - CRUD plus workflow-specific routes (cash/trade/transfer posting, listings) were wired with `Result` to `TypedResults` translation helpers.
   - Input validation continues to live inside the services; controllers remain thin wrappers as planned.

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
