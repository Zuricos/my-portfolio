namespace Zuricos.Folio.Api.Application.Contracts.Portfolios;

/// <summary>
/// Payload for updating mutable portfolio attributes.
/// </summary>
public sealed record PortfolioUpdateCommand(
  string Name,
  string DisplayCurrency,
  string? Institution,
  string? Notes
);
