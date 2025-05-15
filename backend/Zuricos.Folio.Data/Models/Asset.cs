using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class Asset
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required string Isin { get; set; }
  public required string Name { get; set; }
  public required string Symbol { get; set; }
  public required string Currency { get; set; }
  public required DataSource DataSource { get; set; }
  public required DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  public required DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

  // Navigation properties
  public List<AssetHistory> History { get; set; } = [];
}