using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class Account
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;
  public Guid PortfolioId { get; set; }
  public required string Name { get; set; }
  public AccountType Type { get; set; } = AccountType.Cash;
  public required string DisplayCurrency { get; set; }
  public string? Category { get; set; }
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
  public bool IsDeleted { get; set; }
  public DateTimeOffset? DeletedUtc { get; set; }

  public Portfolio? Portfolio { get; set; }

  // Navigation properties
  public List<Activity> Activities { get; set; } = [];
}
