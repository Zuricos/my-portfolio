using Zuricos.Folio.Api.Application.Contracts.Activities;
using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Mappers;

/// <summary>
/// Activity mapping helpers for DTO projection.
/// </summary>
public static class ActivityMappingExtensions
{
  public static ActivitySummaryDto ToSummaryDto(this Activity entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    return new ActivitySummaryDto(
      entity.Id,
      entity.AccountId,
      entity.AssetId,
      entity.Type,
      entity.Quantity,
      entity.Amount,
      entity.Currency,
      entity.FxRate,
      entity.SourceCurrency,
      entity.IsDeleted,
      entity.OccurredOn,
      entity.CreatedUtc
    );
  }

  public static ActivityDetailsDto ToDetailsDto(this Activity entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    MoneyAmountDto settled = new(entity.Amount, entity.Currency);
    MoneyAmountDto? source =
      entity.SourceAmount is { } sourceAmount && entity.SourceCurrency is { } sourceCurrency
        ? new MoneyAmountDto(sourceAmount, sourceCurrency)
        : null;

    return new ActivityDetailsDto(
      entity.Id,
      entity.AccountId,
      entity.AssetId,
      entity.Type,
      entity.Quantity,
      settled,
      source,
      entity.FxRate,
      entity.Tax,
      entity.Fees,
      entity.Description,
      entity.TransferGroupId,
      entity.IsDeleted,
      entity.OccurredOn,
      entity.CreatedUtc,
      entity.UpdatedUtc
    );
  }
}
