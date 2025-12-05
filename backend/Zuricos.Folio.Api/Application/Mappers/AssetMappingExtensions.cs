using Zuricos.Folio.Api.Application.Contracts.Assets;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Mappers;

/// <summary>
/// Asset mapping helpers.
/// </summary>
public static class AssetMappingExtensions
{
  public static AssetSummaryDto ToSummaryDto(this Asset entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    return new AssetSummaryDto(
      entity.Id,
      entity.Isin,
      entity.Name,
      entity.Symbol,
      entity.Currency,
      entity.DataSource,
      entity.IsDeleted,
      entity.CreatedUtc,
      entity.UpdatedUtc
    );
  }

  public static AssetDetailsDto ToDetailsDto(this Asset entity, AssetPriceSnapshotDto? latestPrice)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    return new AssetDetailsDto(
      entity.Id,
      entity.Isin,
      entity.Name,
      entity.Symbol,
      entity.Currency,
      entity.DataSource,
      entity.IsDeleted,
      entity.CreatedUtc,
      entity.UpdatedUtc,
      latestPrice
    );
  }
}
