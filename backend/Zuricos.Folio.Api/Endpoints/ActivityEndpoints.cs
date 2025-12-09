using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Activities;
using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Endpoints;

public static class ActivityEndpoints
{
  private const string GetActivityRouteName = "Activities_GetById";

  public static RouteGroupBuilder MapActivityEndpoints(this RouteGroupBuilder apiGroup)
  {
    RouteGroupBuilder group = apiGroup.MapGroup("activities").WithTags("Activities");

    group
      .MapPost(
        "/cash",
        async (
          IActivityService service,
          CashActivityCreateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<ActivityDetailsDto> result = await service.PostCashActivityAsync(
            userId,
            command,
            cancellationToken
          );
          return result.ToCreated(GetActivityRouteName, dto => new { activityId = dto.Id });
        }
      )
      .WithName("Activities_PostCash")
      .WithOpenApi();

    group
      .MapPost(
        "/trades",
        async (
          IActivityService service,
          AssetTradeCreateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<ActivityDetailsDto> result = await service.PostAssetTradeAsync(
            userId,
            command,
            cancellationToken
          );
          return result.ToCreated(GetActivityRouteName, dto => new { activityId = dto.Id });
        }
      )
      .WithName("Activities_PostTrade")
      .WithOpenApi();

    group
      .MapPost(
        "/transfers",
        async (
          IActivityService service,
          TransferCreateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<IReadOnlyList<ActivityDetailsDto>> result = await service.PostTransferAsync(
            userId,
            command,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName("Activities_PostTransfer")
      .WithOpenApi();

    group
      .MapGet(
        "/{activityId:guid}",
        async (IActivityService service, Guid activityId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<ActivityDetailsDto> result = await service.GetActivityAsync(
            userId,
            activityId,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName(GetActivityRouteName)
      .WithOpenApi();

    apiGroup
      .MapGet(
        "accounts/{accountId:guid}/activities",
        async (
          IActivityService service,
          Guid accountId,
          ActivityType? type,
          Guid? assetId,
          DateTimeOffset? occurredFrom,
          DateTimeOffset? occurredTo,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<IReadOnlyList<ActivitySummaryDto>> result = await service.ListActivitiesAsync(
            userId,
            accountId,
            type,
            assetId,
            occurredFrom,
            occurredTo,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithTags("Activities")
      .WithName("Activities_ListByAccount")
      .WithOpenApi();

    return apiGroup;
  }
}
