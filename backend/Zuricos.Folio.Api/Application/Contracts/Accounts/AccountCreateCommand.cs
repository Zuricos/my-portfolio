using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Accounts;

/// <summary>
/// Payload for creating a new account within a portfolio.
/// </summary>
public sealed record AccountCreateCommand(
  Guid PortfolioId,
  string Name,
  AccountType Type,
  string DisplayCurrency,
  string? Category
);
