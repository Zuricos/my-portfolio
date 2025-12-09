using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Assets;
using Zuricos.Folio.Data.Const;

namespace Zuricos.Folio.Api.Endpoints;

public static class AssetEndpoints
{
  private const string GetAssetRouteName = "Assets_GetById";

  public static RouteGroupBuilder MapAssetEndpoints(this RouteGroupBuilder apiGroup)
  {
    RouteGroupBuilder group = apiGroup.MapGroup("assets").WithTags("Assets");

    group
      .MapPost(
        "/",
        async (
          IAssetCatalogService service,
          AssetCreateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<AssetDetailsDto> result = await service.CreateAssetAsync(
            userId,
            command,
            cancellationToken
          );
          return result.ToCreated(GetAssetRouteName, dto => new { assetId = dto.Id });
        }
      )
      .WithName("Assets_Create")
      .WithOpenApi();

    group
      .MapPut(
        "/{assetId:guid}",
        async (
          IAssetCatalogService service,
          Guid assetId,
          AssetUpdateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<AssetDetailsDto> result = await service.UpdateAssetAsync(
            userId,
            assetId,
            command,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName("Assets_Update")
      .WithOpenApi();

    group
      .MapDelete(
        "/{assetId:guid}",
        async (IAssetCatalogService service, Guid assetId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result result = await service.ArchiveAssetAsync(userId, assetId, cancellationToken);
          return result.ToNoContent();
        }
      )
      .WithName("Assets_Delete")
      .WithOpenApi();

    group
      .MapGet(
        "/{assetId:guid}",
        async (IAssetCatalogService service, Guid assetId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<AssetDetailsDto> result = await service.GetAssetAsync(
            userId,
            assetId,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName(GetAssetRouteName)
      .WithOpenApi();

    group
      .MapGet(
        "/",
        async (IAssetCatalogService service, string? search, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<IReadOnlyList<AssetSummaryDto>> result = await service.ListAssetsAsync(
            userId,
            search,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName("Assets_List")
      .WithOpenApi();

    return apiGroup;
  }
}
