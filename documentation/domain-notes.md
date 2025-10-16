# Domain Notes

- The backend currently operates single-tenant across a placeholder user (`Guid.Empty`), enforced via `ConstValues.DefaultUserId`.
- A `Portfolio` carries one display currency; individual accounts may override their display currency but retain the parent link for reporting.
- Cross-currency and cash-only flows use `Activity` FX fields (`BookCurrency`, `CounterCurrency`, `FxRate`, `SettledAmount`) with paired entries for internal transfers.
- Soft-delete metadata (`IsDeleted`, `DeletedUtc`) and `CreatedUtc`/`UpdatedUtc` timestamps are standard across `Portfolio`, `Account`, `Activity`, `Asset`, and `AssetHistory` entities.
- Numeric precision aligns with PostgreSQL via `ConstValues` aliases: `MoneyColumnType`, `QuantityColumnType`, and `FxRateColumnType`.
