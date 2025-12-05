using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Activities;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Abstractions;

/// <summary>
/// Service boundary for transaction orchestration.
/// </summary>
public interface IActivityService
{
  Task<Result<ActivityDetailsDto>> PostCashActivityAsync(
    Guid userId,
    CashActivityCreateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result<ActivityDetailsDto>> PostAssetTradeAsync(
    Guid userId,
    AssetTradeCreateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result<IReadOnlyList<ActivityDetailsDto>>> PostTransferAsync(
    Guid userId,
    TransferCreateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result<ActivityDetailsDto>> GetActivityAsync(
    Guid userId,
    Guid activityId,
    CancellationToken cancellationToken = default
  );

  Task<Result<IReadOnlyList<ActivitySummaryDto>>> ListActivitiesAsync(
    Guid userId,
    Guid accountId,
    ActivityType? type,
    Guid? assetId,
    DateTimeOffset? occurredFrom,
    DateTimeOffset? occurredTo,
    CancellationToken cancellationToken = default
  );
}
