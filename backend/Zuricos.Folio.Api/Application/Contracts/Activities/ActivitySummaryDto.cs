using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Activities;

/// <summary>
/// Lightweight activity projection for listings.
/// </summary>
public sealed record ActivitySummaryDto(
  Guid Id,
  Guid AccountId,
  Guid? AssetId,
  ActivityType Type,
  decimal Quantity,
  decimal Amount,
  string Currency,
  decimal? FxRate,
  string? SourceCurrency,
  bool IsDeleted,
  DateTimeOffset OccurredOn,
  DateTimeOffset CreatedUtc
);
