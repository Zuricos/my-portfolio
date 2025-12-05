using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Activities;

/// <summary>
/// Payload for trades involving assets (buy, sell, dividend reinvest, etc.).
/// </summary>
public sealed record AssetTradeCreateCommand(
  Guid AccountId,
  Guid AssetId,
  ActivityType Type,
  decimal Quantity,
  decimal Amount,
  string Currency,
  decimal? FxRate,
  string? SourceCurrency,
  decimal? Tax,
  decimal? Fees,
  string? Description,
  DateTimeOffset OccurredOn
);
