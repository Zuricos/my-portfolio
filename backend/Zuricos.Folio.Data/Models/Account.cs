using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

/// <summary>
/// Represents a financial account within a portfolio (e.g., cash account, securities account).
/// Accounts inherit display currency from their parent portfolio but may override it.
/// Supports both cash-only flows and asset transactions.
/// </summary>
public class Account
{
  /// <summary>
  /// Unique identifier for the account.
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// User who owns this account. Defaults to Guid.Empty for single-tenant mode.
  /// </summary>
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;

  /// <summary>
  /// Portfolio that owns this account. Required relationship.
  /// </summary>
  public Guid PortfolioId { get; set; }

  /// <summary>
  /// Display name for the account (e.g., "USD Cash", "Stock Holdings").
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Type of account determining its primary purpose.
  /// </summary>
  public AccountType Type { get; set; } = AccountType.Cash;

  /// <summary>
  /// Display currency for this account. Must be ISO 4217 uppercase.
  /// May differ from parent portfolio's display currency for reporting purposes.
  /// </summary>
  public required string DisplayCurrency { get; set; }

  /// <summary>
  /// Optional categorization for the account (e.g., "Trading", "Retirement").
  /// </summary>
  public string? Category { get; set; }

  /// <summary>
  /// UTC timestamp when the account was created.
  /// </summary>
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// UTC timestamp when the account was last updated.
  /// </summary>
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Soft delete flag. Account deletion may trigger cascade effects on activities.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// UTC timestamp when the account was soft-deleted, if applicable.
  /// </summary>
  public DateTimeOffset? DeletedUtc { get; set; }

  /// <summary>
  /// Parent portfolio reference.
  /// </summary>
  public Portfolio? Portfolio { get; set; }

  // Navigation properties
  /// <summary>
  /// Collection of financial activities (transactions) associated with this account.
  /// </summary>
  public List<Activity> Activities { get; set; } = [];
}
