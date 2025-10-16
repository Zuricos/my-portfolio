using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class User
{
  public Guid Id { get; set; } = ConstValues.DefaultUserId;
  public string Language { get; set; } = "en-US";
  public Theme Theme { get; set; } = Theme.System;
  public string BaseCurrency { get; set; } = "USD";

  // Navigation properties
  public List<Portfolio> Portfolios { get; set; } = [];
  public List<Account> Accounts { get; set; } = [];
  public List<Activity> Activities { get; set; } = [];
}
