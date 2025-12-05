using Zuricos.Folio.Api.Application.Contracts.Accounts;
using Zuricos.Folio.Api.Application.Contracts.Portfolios;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Mappers;

/// <summary>
/// Portfolio mapping helpers to translate EF entities to DTOs.
/// </summary>
public static class PortfolioMappingExtensions
{
  public static PortfolioSummaryDto ToSummaryDto(this Portfolio entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    int accountCount = entity.Accounts?.Count(account => !account.IsDeleted) ?? 0;

    return new PortfolioSummaryDto(
      entity.Id,
      entity.Name,
      entity.DisplayCurrency,
      entity.Institution,
      entity.IsDeleted,
      entity.CreatedUtc,
      entity.UpdatedUtc,
      accountCount
    );
  }

  public static PortfolioDetailsDto ToDetailsDto(this Portfolio entity)
  {
    if (entity is null)
    {
      throw new ArgumentNullException(nameof(entity));
    }

    IReadOnlyList<AccountOutlineDto> accounts = entity.Accounts is null
      ? Array.Empty<AccountOutlineDto>()
      : entity.Accounts.Select(account => account.ToOutlineDto()).ToList();

    return new PortfolioDetailsDto(
      entity.Id,
      entity.Name,
      entity.DisplayCurrency,
      entity.Institution,
      entity.Notes,
      entity.IsDeleted,
      entity.CreatedUtc,
      entity.UpdatedUtc,
      accounts
    );
  }
}
