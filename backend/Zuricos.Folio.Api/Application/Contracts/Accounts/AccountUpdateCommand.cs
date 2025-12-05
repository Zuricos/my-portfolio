using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Accounts;

/// <summary>
/// Payload for updating mutable account fields.
/// </summary>
public sealed record AccountUpdateCommand(
  string Name,
  AccountType Type,
  string DisplayCurrency,
  string? Category
);
