using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Contracts.Assets;

/// <summary>
/// Payload for updating asset metadata.
/// </summary>
public sealed record AssetUpdateCommand(string Name, string Currency, DataSource DataSource);
