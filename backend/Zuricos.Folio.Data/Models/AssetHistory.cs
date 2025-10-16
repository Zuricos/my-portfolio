namespace Zuricos.Folio.Data.Models;

/// <summary>
/// Represents historical price data for a financial asset on a specific date.
/// Used for portfolio valuation and performance tracking.
/// Data is sourced from external providers (CoinGecko, Yahoo Finance, etc.).
/// </summary>
public class AssetHistory
{
  /// <summary>
  /// Composite identifier typically combining AssetId and Date.
  /// </summary>
  public required string Id { get; set; }

  /// <summary>
  /// Asset this price data belongs to. Required relationship.
  /// </summary>
  public required Guid AssetId { get; set; }

  /// <summary>
  /// UTC timestamp when this price record was created.
  /// </summary>
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// UTC timestamp when this price record was last updated.
  /// </summary>
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Trading date for this price data (business date, not system date).
  /// </summary>
  public required DateOnly Date { get; set; }

  /// <summary>
  /// Opening price at market open.
  /// </summary>
  public required decimal Open { get; set; }

  /// <summary>
  /// Highest price during the trading period.
  /// </summary>
  public required decimal High { get; set; }

  /// <summary>
  /// Lowest price during the trading period.
  /// </summary>
  public required decimal Low { get; set; }

  /// <summary>
  /// Closing price at market close.
  /// </summary>
  public required decimal Close { get; set; }

  /// <summary>
  /// Closing price adjusted for dividends, splits, and other corporate actions.
  /// </summary>
  public required decimal AdjustedClose { get; set; }

  /// <summary>
  /// Trading volume for the period.
  /// </summary>
  public required decimal Volume { get; set; }

  /// <summary>
  /// Soft delete flag.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// UTC timestamp when this price record was soft-deleted, if applicable.
  /// </summary>
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  /// <summary>
  /// Asset reference this price data belongs to.
  /// </summary>
  public Asset? Asset { get; set; }
}
