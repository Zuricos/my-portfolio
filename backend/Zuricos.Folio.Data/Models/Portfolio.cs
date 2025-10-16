using Zuricos.Folio.Data.Const;

namespace Zuricos.Folio.Data.Models;

/// <summary>
/// Represents a financial portfolio aggregate (e.g., broker, bank) that owns currency-specific accounts.
/// Each portfolio has a single display currency but accounts may override with their own display currency.
/// Portfolios cannot be soft-deleted while active accounts remain.
/// </summary>
public class Portfolio
{
  /// <summary>
  /// Unique identifier for the portfolio.
  /// </summary>
  public Guid Id { get; set; } = Guid.NewGuid();

  /// <summary>
  /// User who owns this portfolio. Defaults to Guid.Empty for single-tenant mode until user management is implemented.
  /// </summary>
  public Guid UserId { get; set; } = ConstValues.DefaultUserId;

  /// <summary>
  /// Display name for the portfolio (e.g., "Interactive Brokers", "Chase Savings").
  /// </summary>
  public required string Name { get; set; }

  /// <summary>
  /// Optional financial institution name (e.g., "Interactive Brokers LLC").
  /// </summary>
  public string? Institution { get; set; }

  /// <summary>
  /// Default display currency for the portfolio. Must be ISO 4217 three-letter uppercase code (e.g., "USD", "EUR").
  /// Individual accounts may override this for their own display purposes.
  /// </summary>
  public required string DisplayCurrency { get; set; }

  /// <summary>
  /// Optional notes or description for the portfolio.
  /// </summary>
  public string? Notes { get; set; }

  /// <summary>
  /// UTC timestamp when the portfolio was created.
  /// </summary>
  public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// UTC timestamp when the portfolio was last updated.
  /// </summary>
  public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Soft delete flag. Portfolio cannot be deleted while it has non-deleted accounts.
  /// </summary>
  public bool IsDeleted { get; set; }

  /// <summary>
  /// UTC timestamp when the portfolio was soft-deleted, if applicable.
  /// </summary>
  public DateTimeOffset? DeletedUtc { get; set; }

  // Navigation properties
  /// <summary>
  /// Collection of accounts belonging to this portfolio.
  /// </summary>
  public List<Account> Accounts { get; set; } = [];
}
