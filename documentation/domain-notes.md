# Domain Notes

## Single-Tenant Architecture
- The backend currently operates single-tenant across a placeholder user (`Guid.Empty`), enforced via `ConstValues.DefaultUserId`.
- All entities (`Portfolio`, `Account`, `Activity`, `Asset`, `AssetHistory`) are scoped to this user until authentication is implemented.
- Migration to multi-user will require updating existing data to proper user associations.

## Portfolio Aggregate Design
- A `Portfolio` carries one display currency; individual accounts may override their display currency but retain the parent link for reporting.
- Cross-currency and cash-only flows use `Activity` FX fields (`BookCurrency`, `CounterCurrency`, `FxRate`, `SettledAmount`) with paired entries for internal transfers.
- Soft-delete metadata (`IsDeleted`, `DeletedUtc`) and `CreatedUtc`/`UpdatedUtc` timestamps are standard across all entities.

## Domain Invariants

### Portfolio Rules
- **Currency Format**: `DisplayCurrency` must be ISO 4217 three-letter uppercase code (e.g., "USD", "EUR", "GBP")
- **Soft Delete Constraint**: Portfolio cannot be soft-deleted (`IsDeleted = true`) while it has non-deleted accounts
- **Name Uniqueness**: Portfolio names should be unique per user (enforced at application layer)

### Account Rules
- **Portfolio Relationship**: Account must belong to exactly one portfolio (`PortfolioId` required)
- **Currency Inheritance**: `DisplayCurrency` may differ from parent portfolio for reporting, but both must be valid ISO 4217
- **Type Consistency**: Account type should align with expected activities (Cash accounts for deposits/withdrawals, Securities for trades)

### Activity Rules
- **Asset Constraint**: `AssetId` is nullable - required for asset transactions (buy/sell), null for cash flows (deposit/withdrawal/transfer)
- **Transfer Pairing**: Transfer activities between accounts must use paired entries with the same `TransferGroupId`
- **Currency Validation**: All currency fields (`BookCurrency`, `CounterCurrency`, `FeesCurrency`, `UnitPriceCurrency`) must be ISO 4217 format
- **FX Relationship**: When `CounterCurrency` is present, `FxRate` should be provided and `FxRate = CounterAmount / Amount`
- **Timestamp Logic**: `OccurredOn` represents business date, `CreatedUtc` represents system creation time

### Asset Rules
- **Symbol Uniqueness**: Asset symbols must be globally unique across the system
- **Currency Requirement**: Asset `Currency` represents the base currency for pricing from data providers
- **ISIN Format**: ISIN should follow standard 12-character format when available

## Numeric Precision Strategy
Numeric precision aligns with PostgreSQL via `ConstValues` aliases:
- **MoneyColumnType**: `numeric(19, 4)` for prices, amounts, fees, taxes
- **QuantityColumnType**: `numeric(20, 8)` for asset quantities and volumes
- **FxRateColumnType**: `numeric(18, 8)` for high-precision exchange rates
- **ChargeColumnType**: `numeric(19, 4)` for fees and charges (same as money)

## Value Objects Decision
**Decision**: Defer implementation of `Money` and `CurrencyCode` value objects to later phases.

**Rationale**: 
- Current primitive types (decimal + string) provide sufficient functionality for MVP
- Value objects would require additional EF Core configuration for owned entities
- Focus remains on core domain logic and API development
- Can be introduced in future refactoring without breaking existing APIs
- Validation of currency codes can be handled at application/API layer

**Future Considerations**:
- `Money` struct could encapsulate amount + currency + formatting
- `CurrencyCode` could provide validation and conversion utilities
- Consider when type safety becomes critical (likely during frontend integration)
