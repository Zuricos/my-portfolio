using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

/// <summary>
/// Represents a financial activity (transaction) within an account.
/// Supports both cash-only flows (deposits, withdrawals) and asset transactions (buy, sell).
/// Multi-currency transactions are supported via FX fields.
/// Transfers between accounts use paired activities linked by TransferGroupId.
/// </summary>
public class Activity
{
  /// <summary>
  /// Unique identifier for the activity.
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  // Foreign keys
  /// <summary>
  /// User who owns this activity. Defaults to Guid.Empty for single-tenant mode.
  /// </summary>
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;

  /// <summary>
  /// Account where this activity occurred. Required relationship.
  /// </summary>
  public required Guid AccountId { get; set; }

  /// <summary>
  /// Asset involved in this activity. Null for cash-only transactions (deposits, withdrawals, transfers).
  /// </summary>
  public Guid? AssetId { get; set; }

  // Activity details
  /// <summary>
  /// Type of activity (deposit, withdrawal, buy, sell, transfer, dividend, etc.).
  /// </summary>
  public ActivityType Type { get; set; } = ActivityType.Other;

  /// <summary>
  /// Quantity of asset units involved. Zero for cash-only activities.
  /// </summary>
  public decimal Quantity { get; set; }

  /// <summary>
  /// Primary monetary amount in the book currency.
  /// </summary>
  public decimal Amount { get; set; }

  /// <summary>
  /// Currency of the primary amount. Must be ISO 4217 uppercase.
  /// </summary>
  public required string BookCurrency { get; set; }

  /// <summary>
  /// Optional counter-party amount for FX transactions.
  /// </summary>
  public decimal? CounterAmount { get; set; }

  /// <summary>
  /// Currency of the counter amount for FX transactions.
  /// </summary>
  public string? CounterCurrency { get; set; }

  /// <summary>
  /// Exchange rate used for currency conversion (CounterAmount / Amount).
  /// </summary>
  public decimal? FxRate { get; set; }

  /// <summary>
  /// Final settled amount after FX conversion and fees.
  /// </summary>
  public decimal? SettledAmount { get; set; }

  /// <summary>
  /// Price per unit of the asset at transaction time.
  /// </summary>
  public decimal? UnitPrice { get; set; }

  /// <summary>
  /// Currency denomination of the unit price.
  /// </summary>
  public string? UnitPriceCurrency { get; set; }

  /// <summary>
  /// Tax amount charged for this activity.
  /// </summary>
  public decimal? Tax { get; set; }

  /// <summary>
  /// Fee amount charged for this activity.
  /// </summary>
  public decimal? Fees { get; set; }

  /// <summary>
  /// Currency denomination of fees and taxes.
  /// </summary>
  public string? FeesCurrency { get; set; }

  /// <summary>
  /// Optional description or notes for the activity.
  /// </summary>
  public string? Description { get; set; }

  // Properties for transfer activities
  /// <summary>
  /// Groups related transfer activities together.
  /// Transfers between accounts use paired activities with the same TransferGroupId.
  /// </summary>
  public Guid? TransferGroupId { get; set; }

  // Timestamps
  /// <summary>
  /// When the financial activity actually occurred (business date).
  /// </summary>
  public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// UTC timestamp when the activity record was created in the system.
  /// </summary>
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// UTC timestamp when the activity was last updated.
  /// </summary>
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Soft delete flag.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// UTC timestamp when the activity was soft-deleted, if applicable.
  /// </summary>
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  /// <summary>
  /// Account reference where this activity occurred.
  /// </summary>
  public Account? Account { get; set; }

  /// <summary>
  /// Asset reference involved in this activity, if applicable.
  /// </summary>
  public Asset? Asset { get; set; }
}
