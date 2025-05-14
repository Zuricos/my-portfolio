using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Data.Models;

public class UserSettings
{
  public required Guid UserId { get; set; } = Guid.NewGuid();
  public required string Language { get; set; } = "en-US";
  public Theme Theme { get; set; } = Theme.System;
  public required string BaseCurrency { get; set; } = "USD";
}