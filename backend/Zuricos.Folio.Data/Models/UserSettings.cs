using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class User
{
  public required Guid Id { get; set; } = Guid.NewGuid();
  public required string Language { get; set; } = "en-US";
  public Theme Theme { get; set; } = Theme.System;
  public required string BaseCurrency { get; set; } = "USD";
  
  // Navigation properties
  public List<Account> Accounts { get; set; } = [];
  public List<Activity> Activities { get; set; } = [];
}