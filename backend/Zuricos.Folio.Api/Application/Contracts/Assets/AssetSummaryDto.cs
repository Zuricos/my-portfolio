using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Assets;

/// <summary>
/// Summary projection for asset listings.
/// </summary>
public sealed record AssetSummaryDto(
  Guid Id,
  string Isin,
  string Name,
  string Symbol,
  string Currency,
  DataSource DataSource,
  bool IsDeleted,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc
);
