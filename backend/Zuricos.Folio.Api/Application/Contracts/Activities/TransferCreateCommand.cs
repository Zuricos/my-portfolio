namespace Zuricos.Folio.Api.Application.Contracts.Activities;

/// <summary>
/// Payload for inter-account transfers modeled as paired activities.
/// </summary>
public sealed record TransferCreateCommand(
  Guid SourceAccountId,
  Guid DestinationAccountId,
  decimal SourceAmount,
  string SourceCurrency,
  decimal DestinationAmount,
  string DestinationCurrency,
  decimal? FxRate,
  decimal? Fees,
  string? Description,
  DateTimeOffset OccurredOn
);
