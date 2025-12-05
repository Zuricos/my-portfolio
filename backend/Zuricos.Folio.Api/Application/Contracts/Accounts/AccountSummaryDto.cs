using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Accounts;

/// <summary>
/// Summary projection for account listings.
/// </summary>
public sealed record AccountSummaryDto(
  Guid Id,
  Guid PortfolioId,
  string Name,
  AccountType Type,
  string DisplayCurrency,
  string? Category,
  bool IsDeleted,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc
);
