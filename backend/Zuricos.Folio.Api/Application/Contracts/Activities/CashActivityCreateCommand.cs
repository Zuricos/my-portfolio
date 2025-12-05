using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Activities;

/// <summary>
/// Payload for cash-only activities (deposit, withdrawal, fee, tax).
/// </summary>
public sealed record CashActivityCreateCommand(
  Guid AccountId,
  ActivityType Type,
  decimal Amount,
  string Currency,
  decimal? FxRate,
  string? SourceCurrency,
  decimal? Tax,
  decimal? Fees,
  string? Description,
  DateTimeOffset OccurredOn
);
