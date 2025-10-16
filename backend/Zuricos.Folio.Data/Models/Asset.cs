using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

/// <summary>
/// Represents a tradeable financial instrument (stock, bond, cryptocurrency, etc.).
/// Assets are shared across users but scoped for single-tenant mode via UserId.
/// Symbol must be unique across the system.
/// </summary>
public class Asset
{
  /// <summary>
  /// Unique identifier for the asset.
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// User scope for this asset. Defaults to Guid.Empty for single-tenant mode.
  /// </summary>
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;

  /// <summary>
  /// International Securities Identification Number (12 characters).
  /// </summary>
  public required string Isin { get; set; }

  /// <summary>
  /// Full name of the asset (e.g., "Apple Inc.", "Bitcoin").
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Trading symbol or ticker (e.g., "AAPL", "BTC"). Must be unique.
  /// </summary>
  public required string Symbol { get; set; }

  /// <summary>
  /// Base currency for asset pricing. Must be ISO 4217 uppercase.
  /// </summary>
  public required string Currency { get; set; }

  /// <summary>
  /// Data provider used for price feeds (CoinGecko, Yahoo Finance, etc.).
  /// </summary>
  public required DataSource DataSource { get; set; }

  /// <summary>
  /// UTC timestamp when the asset was created.
  /// </summary>
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// UTC timestamp when the asset was last updated.
  /// </summary>
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Soft delete flag.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// UTC timestamp when the asset was soft-deleted, if applicable.
  /// </summary>
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  /// <summary>
  /// Historical price data for this asset.
  /// </summary>
  public List<AssetHistory> History { get; set; } = [];
}
