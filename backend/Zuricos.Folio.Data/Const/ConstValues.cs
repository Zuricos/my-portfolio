namespace Zuricos.Folio.Data.Const;

public static class ConstValues
{
  // Column type aliases for PostgreSQL numeric columns used across monetary fields
  public const string MoneyColumnType = "numeric(19, 4)";
  public const string QuantityColumnType = "numeric(20, 8)";
  public const string FxRateColumnType = "numeric(18, 8)";
  public const string ChargeColumnType = "numeric(19, 4)";

  // Placeholder user binding until user management is introduced
  public static readonly Guid DefaultUserId = Guid.Empty;
}
