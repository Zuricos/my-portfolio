namespace Zuricos.Folio.Data.Const;

/// <summary>
/// Constant values used throughout the domain layer for database configuration and default values.
/// </summary>
public static class ConstValues
{
  /// <summary>
  /// PostgreSQL column type for monetary amounts with 4 decimal places (e.g., prices, fees, taxes).
  /// Format: numeric(19, 4) supporting values up to 999,999,999,999,999.9999
  /// </summary>
  public const string MoneyColumnType = "numeric(19, 4)";

  /// <summary>
  /// PostgreSQL column type for asset quantities with 8 decimal places (e.g., share counts, crypto amounts).
  /// Format: numeric(20, 8) supporting precise fractional holdings
  /// </summary>
  public const string QuantityColumnType = "numeric(20, 8)";

  /// <summary>
  /// PostgreSQL column type for foreign exchange rates with 8 decimal places.
  /// Format: numeric(18, 8) providing high precision for currency conversions
  /// </summary>
  public const string FxRateColumnType = "numeric(18, 8)";

  /// <summary>
  /// PostgreSQL column type for charges (fees, taxes) with 4 decimal places.
  /// Format: numeric(19, 4) - same as MoneyColumnType for consistency
  /// </summary>
  public const string ChargeColumnType = "numeric(19, 4)";

  /// <summary>
  /// Placeholder user identifier for single-tenant operation until user management is implemented.
  /// All portfolios, accounts, and activities are associated with this user by default.
  /// </summary>
  public static readonly Guid DefaultUserId = Guid.Empty;
}
