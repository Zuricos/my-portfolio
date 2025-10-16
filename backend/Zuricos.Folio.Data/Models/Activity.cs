using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class Activity
{
  // ID
  public Guid Id { get; set; } = Guid.NewGuid();

  // Foreign keys
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;
  public required Guid AccountId { get; set; }
  public Guid? AssetId { get; set; }

  // Activity details
  public ActivityType Type { get; set; } = ActivityType.Other;
  public decimal Quantity { get; set; }
  public decimal Amount { get; set; }
  public required string BookCurrency { get; set; }
  public decimal? CounterAmount { get; set; }
  public string? CounterCurrency { get; set; }
  public decimal? FxRate { get; set; }
  public decimal? SettledAmount { get; set; }
  public decimal? UnitPrice { get; set; }
  public string? UnitPriceCurrency { get; set; }
  public decimal? Tax { get; set; }
  public decimal? Fees { get; set; }
  public string? FeesCurrency { get; set; }
  public string? Description { get; set; }

  // Properties for transfer activities
  public Guid? TransferGroupId { get; set; }

  // Timestamps
  public DateTimeOffset OccurredOn { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  public Account? Account { get; set; }
  public Asset? Asset { get; set; }
}
