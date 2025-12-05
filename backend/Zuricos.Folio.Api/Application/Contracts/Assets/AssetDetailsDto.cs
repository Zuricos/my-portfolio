using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Assets;

/// <summary>
/// Detailed asset projection including price snapshot data.
/// </summary>
public sealed record AssetDetailsDto(
  Guid Id,
  string Isin,
  string Name,
  string Symbol,
  string Currency,
  DataSource DataSource,
  bool IsDeleted,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc,
  AssetPriceSnapshotDto? LatestPrice
);

/// <summary>
/// Latest price information for an asset.
/// </summary>
public sealed record AssetPriceSnapshotDto(
  MoneyAmountDto Price,
  DateTimeOffset AsOf,
  string Source
);
