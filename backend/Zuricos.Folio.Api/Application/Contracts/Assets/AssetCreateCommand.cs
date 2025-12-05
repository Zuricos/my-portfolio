using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Assets;

/// <summary>
/// Payload for creating a new asset instrument.
/// </summary>
public sealed record AssetCreateCommand(
  string Isin,
  string Name,
  string Symbol,
  string Currency,
  DataSource DataSource
);
