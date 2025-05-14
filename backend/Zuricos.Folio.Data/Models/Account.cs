using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;
public class Account
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public required Guid UserId { get; set; }
  public required string Name { get; set; }
  public required AccountType Type { get; set; }
  public required string Currency { get; set; }
  public required decimal Balance { get; set; }
  public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}