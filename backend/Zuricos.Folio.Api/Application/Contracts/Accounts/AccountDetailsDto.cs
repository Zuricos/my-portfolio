using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Accounts;

/// <summary>
/// Detailed account representation containing latest balance snapshots.
/// </summary>
public sealed record AccountDetailsDto(
  Guid Id,
  Guid PortfolioId,
  string Name,
  AccountType Type,
  string DisplayCurrency,
  string? Category,
  bool IsDeleted,
  DateTimeOffset CreatedUtc,
  DateTimeOffset UpdatedUtc,
  MoneyAmountDto BookBalance,
  IReadOnlyList<AccountAssetPositionDto> Positions
);

/// <summary>
/// Asset holding snapshot for an account.
/// </summary>
public sealed record AccountAssetPositionDto(
  Guid AssetId,
  string Symbol,
  decimal Quantity,
  MoneyAmountDto CostBasis,
  MoneyAmountDto? MarketValue
);
