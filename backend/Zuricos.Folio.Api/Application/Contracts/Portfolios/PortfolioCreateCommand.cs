namespace Zuricos.Folio.Api.Application.Contracts.Portfolios;

/// <summary>
/// Payload for creating a new portfolio.
/// </summary>
public sealed record PortfolioCreateCommand(
  string Name,
  string DisplayCurrency,
  string? Institution,
  string? Notes
);
