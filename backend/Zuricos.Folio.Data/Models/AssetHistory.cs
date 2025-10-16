namespace Zuricos.Folio.Data.Models;

public class AssetHistory
{
  public required string Id { get; set; }
  public required Guid AssetId { get; set; }
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public required DateOnly Date { get; set; }
  public required decimal Open { get; set; }
  public required decimal High { get; set; }
  public required decimal Low { get; set; }
  public required decimal Close { get; set; }
  public required decimal AdjustedClose { get; set; }
  public required decimal Volume { get; set; }
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  public Asset? Asset { get; set; }
}
