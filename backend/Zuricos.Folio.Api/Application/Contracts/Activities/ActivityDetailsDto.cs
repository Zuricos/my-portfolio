using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Activities;

/// <summary>
/// Full detail for a single activity, including computed amounts.
/// </summary>
public sealed record ActivityDetailsDto(
  Guid Id,
  Guid AccountId,
  Guid? AssetId,
  ActivityType Type,
  decimal Quantity,
  MoneyAmountDto SettledAmount,
  MoneyAmountDto? SourceAmount,
  decimal? FxRate,
  decimal? Tax,
  decimal? Fees,
  string? Description,
  Guid? TransferGroupId,
  bool IsDeleted,
  DateTimeOffset OccurredOn,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc
);
