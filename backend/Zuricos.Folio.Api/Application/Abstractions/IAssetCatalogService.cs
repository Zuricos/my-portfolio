using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Assets;

namespace Zuricos.Folio.Api.Application.Abstractions;

/// <summary>
/// Service boundary for asset catalog management.
/// </summary>
public interface IAssetCatalogService
{
  Task<Result<AssetDetailsDto>> CreateAssetAsync(
    Guid userId,
    AssetCreateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result<AssetDetailsDto>> UpdateAssetAsync(
    Guid userId,
    Guid assetId,
    AssetUpdateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result> ArchiveAssetAsync(
    Guid userId,
    Guid assetId,
    CancellationToken cancellationToken = default
  );

  Task<Result<AssetDetailsDto>> GetAssetAsync(
    Guid userId,
    Guid assetId,
    CancellationToken cancellationToken = default
  );

  Task<Result<IReadOnlyList<AssetSummaryDto>>> ListAssetsAsync(
    Guid userId,
    string? search,
    CancellationToken cancellationToken = default
  );

  Task<Result> EnsureSeedAssetsAsync(Guid userId, CancellationToken cancellationToken = default);
}
