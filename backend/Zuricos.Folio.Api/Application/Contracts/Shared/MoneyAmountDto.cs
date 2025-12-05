namespace Zuricos.Folio.Api.Application.Contracts.Shared;

/// <summary>
/// Represents a monetary value paired with its ISO currency code.
/// </summary>
public sealed record MoneyAmountDto(decimal Amount, string Currency);
