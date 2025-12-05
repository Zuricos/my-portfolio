using Zuricos.Folio.Api.Application.Contracts.Accounts;
using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Mappers;

/// <summary>
/// Account mapping helpers for DTO conversion.
/// </summary>
public static class AccountMappingExtensions
{
  public static AccountOutlineDto ToOutlineDto(this Account entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    return new AccountOutlineDto(
      entity.Id,
      entity.Name,
      entity.DisplayCurrency,
      entity.Type.ToString(),
      entity.IsDeleted
    );
  }

  public static AccountSummaryDto ToSummaryDto(this Account entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    return new AccountSummaryDto(
      entity.Id,
      entity.PortfolioId,
      entity.Name,
      entity.Type,
      entity.DisplayCurrency,
      entity.Category,
      entity.IsDeleted,
      entity.CreatedUtc,
      entity.UpdatedUtc
    );
  }

  public static AccountDetailsDto ToDetailsDto(
    this Account entity,
    MoneyAmountDto bookBalance,
    IReadOnlyList<AccountAssetPositionDto> positions
  )
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    return new AccountDetailsDto(
      entity.Id,
      entity.PortfolioId,
      entity.Name,
      entity.Type,
      entity.DisplayCurrency,
      entity.Category,
      entity.IsDeleted,
      entity.CreatedUtc,
      entity.UpdatedUtc,
      bookBalance,
      positions
    );
  }
}
