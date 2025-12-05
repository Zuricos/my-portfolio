namespace Zuricos.Folio.Api.Application.Contracts.Accounts;

/// <summary>
/// Minimal projection of an account, primarily for portfolio listings.
/// </summary>
public sealed record AccountOutlineDto(
  Guid Id,
  string Name,
  string DisplayCurrency,
  string Type,
  bool IsDeleted
);
