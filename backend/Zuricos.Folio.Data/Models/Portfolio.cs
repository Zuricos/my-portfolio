using Zuricos.Folio.Data.Const;

namespace Zuricos.Folio.Data.Models;

public class Portfolio
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;
  public required string Name { get; set; }
  public string? Institution { get; set; }
  public required string DisplayCurrency { get; set; }
  public string? Notes { get; set; }
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  public List<Account> Accounts { get; set; } = [];
}
