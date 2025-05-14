using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class Activity
{
  // ID
  public Guid Id { get; set; } = Guid.NewGuid();

  // Foreign keys
  public required Guid UserId { get; set; }
  public required Guid AccountId { get; set; }
  public required string AssetId { get; set; }

  // Activity details
  public required ActivityType Type { get; set; }
  public required decimal Amount { get; set; }
  public required decimal Price { get; set; }
  public required string PriceCurrency { get; set; }
  public required decimal Tax { get; set; }
  public required decimal Fees { get; set; }
  public required string FeesCurrency { get; set; }
  public string? Description { get; set; }

  // Properties for transfer activities
  public Guid? TransactionId { get; set; }
  public decimal? ExchangeRate { get; set; }

  // Timestamps
  public required DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public required DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}