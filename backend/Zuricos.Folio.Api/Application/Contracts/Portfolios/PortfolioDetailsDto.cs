using Zuricos.Folio.Api.Application.Contracts.Accounts;

namespace Zuricos.Folio.Api.Application.Contracts.Portfolios;

/// <summary>
/// Detailed portfolio representation including aggregated values.
/// </summary>
public sealed record PortfolioDetailsDto(
  Guid Id,
  string Name,
  string DisplayCurrency,
  string? Institution,
  string? Notes,
  bool IsDeleted,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc,
  IReadOnlyList<AccountOutlineDto> Accounts
);
