using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

/// <summary>
/// Represents user settings and preferences for the portfolio application.
/// Currently configured for single-tenant mode with Guid.Empty as the default user.
/// Will be expanded when multi-user authentication is implemented.
/// </summary>
public class User
{
  /// <summary>
  /// User identifier. Defaults to Guid.Empty for single-tenant mode.
  /// </summary>
  public Guid Id { get; set; } = ConstValues.DefaultUserId;

  /// <summary>
  /// Preferred language/locale (e.g., "en-US", "de-DE").
  /// </summary>
  public string Language { get; set; } = "en-US";

  /// <summary>
  /// UI theme preference (System, Light, Dark).
  /// </summary>
  public Theme Theme { get; set; } = Theme.System;

  /// <summary>
  /// Base currency for portfolio valuation and reporting. Must be ISO 4217 uppercase.
  /// </summary>
  public string BaseCurrency { get; set; } = "USD";

  // Navigation properties
  /// <summary>
  /// Portfolios owned by this user.
  /// </summary>
  public List<Portfolio> Portfolios { get; set; } = [];

  /// <summary>
  /// Accounts owned by this user across all portfolios.
  /// </summary>
  public List<Account> Accounts { get; set; } = [];

  /// <summary>
  /// Activities performed by this user across all accounts.
  /// </summary>
  public List<Activity> Activities { get; set; } = [];
}
