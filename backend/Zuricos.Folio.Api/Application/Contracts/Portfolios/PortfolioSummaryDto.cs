namespace Zuricos.Folio.Api.Application.Contracts.Portfolios;

/// <summary>
/// Lightweight portfolio representation for listings.
/// </summary>
public sealed record PortfolioSummaryDto(
  Guid Id,
  string Name,
  string DisplayCurrency,
  string? Institution,
  bool IsDeleted,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc,
  int AccountCount
);
