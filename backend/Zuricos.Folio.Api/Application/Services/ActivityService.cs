using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Activities;
using Zuricos.Folio.Api.Application.Mappers;
using Zuricos.Folio.Data;
using Zuricos.Folio.Data.Enums;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Services;

/// <summary>
/// Default implementation of <see cref="IActivityService"/>.
/// </summary>
public sealed class ActivityService : IActivityService
{
  private static readonly ActivityType[] CashActivityTypes =
  [
    ActivityType.Deposit,
    ActivityType.Withdrawal,
    ActivityType.Dividend,
    ActivityType.Interest,
    ActivityType.Fee,
    ActivityType.Tax,
    ActivityType.Other,
  ];

  private static readonly ActivityType[] TradeActivityTypes =
  [
    ActivityType.Buy,
    ActivityType.Sell,
    ActivityType.Dividend,
  ];

  private readonly IDbContextFactory<FolioDbContext> _dbContextFactory;
  private readonly ILogger<ActivityService> _logger;

  public ActivityService(
    IDbContextFactory<FolioDbContext> dbContextFactory,
    ILogger<ActivityService> logger
  )
  {
    _dbContextFactory = dbContextFactory;
    _logger = logger;
  }

  public async Task<Result<ActivityDetailsDto>> PostCashActivityAsync(
    Guid userId,
    CashActivityCreateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    if (!CashActivityTypes.Contains(payload.Type))
    {
      return Result<ActivityDetailsDto>.Failure(
        new ValidationError(
          "activity.type_invalid",
          "Activity type is not supported for cash operations."
        )
      );
    }

    ValidationError[] validationErrors = ValidateCommonAmounts(payload.Amount, payload.Currency);
    if (validationErrors.Length > 0)
    {
      return Result<ActivityDetailsDto>.Failure(validationErrors);
    }

    if (payload.FxRate is < 0)
    {
      return Result<ActivityDetailsDto>.Failure(
        new ValidationError("activity.fx_invalid", "FX rate must be positive when provided.")
      );
    }

    if (payload.SourceCurrency is not null && payload.FxRate is null)
    {
      return Result<ActivityDetailsDto>.Failure(
        new ValidationError(
          "activity.fx_required",
          "FX rate is required when source currency is provided."
        )
      );
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );
    await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(
      cancellationToken
    );

    (Account? accountEntity, ValidationError? accountError) = await TryGetActiveAccountAsync(
      context,
      userId,
      payload.AccountId,
      cancellationToken
    );

    if (accountError is not null)
    {
      return Result<ActivityDetailsDto>.Failure(accountError);
    }

    Account account = accountEntity!;
    string currency = payload.Currency.Trim().ToUpperInvariant();
    string? sourceCurrency = payload.SourceCurrency?.Trim().ToUpperInvariant();

    if (!currency.Equals(account.DisplayCurrency, StringComparison.OrdinalIgnoreCase))
    {
      _logger.LogDebug(
        "Cash activity currency {Currency} differs from account display currency {DisplayCurrency} for account {AccountId}.",
        currency,
        account.DisplayCurrency,
        account.Id
      );
    }

    decimal signedAmount = ApplyCashSign(payload.Type, payload.Amount);

    Activity activity = new()
    {
      UserId = userId,
      AccountId = account.Id,
      Type = payload.Type,
      Amount = signedAmount,
      Currency = currency,
      FxRate = sourceCurrency is null ? null : payload.FxRate,
      SourceCurrency = sourceCurrency,
      Tax = payload.Tax,
      Fees = payload.Fees,
      Description = payload.Description?.Trim(),
      OccurredOn = payload.OccurredOn,
      CreatedUtc = DateTimeOffset.UtcNow,
      UpdatedUtc = DateTimeOffset.UtcNow,
    };

    await context.Activities.AddAsync(activity, cancellationToken);
    account.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);
    await transaction.CommitAsync(cancellationToken);

    return Result<ActivityDetailsDto>.Success(activity.ToDetailsDto());
  }

  public async Task<Result<ActivityDetailsDto>> PostAssetTradeAsync(
    Guid userId,
    AssetTradeCreateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    if (!TradeActivityTypes.Contains(payload.Type))
    {
      return Result<ActivityDetailsDto>.Failure(
        new ValidationError(
          "activity.type_invalid",
          "Activity type is not supported for asset trades."
        )
      );
    }

    ValidationError[] validationErrors = ValidateCommonAmounts(payload.Amount, payload.Currency);
    List<ValidationError> errors = [.. validationErrors];

    if (payload.Quantity <= 0)
    {
      errors.Add(new ValidationError("activity.quantity_invalid", "Quantity must be positive."));
    }

    if (payload.FxRate is < 0)
    {
      errors.Add(
        new ValidationError("activity.fx_invalid", "FX rate must be positive when provided.")
      );
    }

    if (errors.Count > 0)
    {
      return Result<ActivityDetailsDto>.Failure(errors);
    }

    if (payload.SourceCurrency is not null && payload.FxRate is null)
    {
      errors.Add(
        new ValidationError(
          "activity.fx_required",
          "FX rate is required when source currency is provided."
        )
      );
      return Result<ActivityDetailsDto>.Failure(errors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );
    await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(
      cancellationToken
    );

    (Account? accountEntity, ValidationError? accountError) = await TryGetActiveAccountAsync(
      context,
      userId,
      payload.AccountId,
      cancellationToken
    );
    if (accountError is not null)
    {
      return Result<ActivityDetailsDto>.Failure(accountError);
    }

    (Asset? assetEntity, ValidationError? assetError) = await TryGetActiveAssetAsync(
      context,
      userId,
      payload.AssetId,
      cancellationToken
    );
    if (assetError is not null)
    {
      return Result<ActivityDetailsDto>.Failure(assetError);
    }

    Account account = accountEntity!;
    Asset asset = assetEntity!;

    string currency = payload.Currency.Trim().ToUpperInvariant();
    string? sourceCurrency = payload.SourceCurrency?.Trim().ToUpperInvariant();

    if (!currency.Equals(account.DisplayCurrency, StringComparison.OrdinalIgnoreCase))
    {
      _logger.LogDebug(
        "Trade activity currency {Currency} differs from account display currency {DisplayCurrency} for account {AccountId}.",
        currency,
        account.DisplayCurrency,
        account.Id
      );
    }

    decimal signedAmount = ApplyTradeSign(payload.Type, payload.Amount);
    decimal signedQuantity = ApplyTradeQuantity(payload.Type, payload.Quantity);

    Activity activity = new()
    {
      UserId = userId,
      AccountId = account.Id,
      AssetId = asset.Id,
      Type = payload.Type,
      Quantity = signedQuantity,
      Amount = signedAmount,
      Currency = currency,
      FxRate = sourceCurrency is null ? null : payload.FxRate,
      SourceCurrency = sourceCurrency,
      Tax = payload.Tax,
      Fees = payload.Fees,
      Description = payload.Description?.Trim(),
      OccurredOn = payload.OccurredOn,
      CreatedUtc = DateTimeOffset.UtcNow,
      UpdatedUtc = DateTimeOffset.UtcNow,
    };

    await context.Activities.AddAsync(activity, cancellationToken);
    account.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);
    await transaction.CommitAsync(cancellationToken);

    await context.Entry(activity).Reference(a => a.Asset).LoadAsync(cancellationToken);

    return Result<ActivityDetailsDto>.Success(activity.ToDetailsDto());
  }

  public async Task<Result<IReadOnlyList<ActivityDetailsDto>>> PostTransferAsync(
    Guid userId,
    TransferCreateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] sourceErrors = ValidateCommonAmounts(
      payload.SourceAmount,
      payload.SourceCurrency
    );
    ValidationError[] destinationErrors = ValidateCommonAmounts(
      payload.DestinationAmount,
      payload.DestinationCurrency
    );
    List<ValidationError> errors = [.. sourceErrors, .. destinationErrors];

    if (payload.SourceAccountId == payload.DestinationAccountId)
    {
      errors.Add(
        new ValidationError(
          "activity.transfer_same_account",
          "Source and destination accounts must differ."
        )
      );
    }

    if (errors.Count > 0)
    {
      return Result<IReadOnlyList<ActivityDetailsDto>>.Failure(errors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    (Account? sourceAccountEntity, ValidationError? sourceAccountError) =
      await TryGetActiveAccountAsync(context, userId, payload.SourceAccountId, cancellationToken);
    if (sourceAccountError is not null)
    {
      errors.Add(sourceAccountError);
    }

    (Account? destinationAccountEntity, ValidationError? destinationAccountError) =
      await TryGetActiveAccountAsync(
        context,
        userId,
        payload.DestinationAccountId,
        cancellationToken
      );
    if (destinationAccountError is not null)
    {
      errors.Add(destinationAccountError);
    }

    if (errors.Count > 0)
    {
      return Result<IReadOnlyList<ActivityDetailsDto>>.Failure(errors);
    }

    Account sourceAccount = sourceAccountEntity!;
    Account destinationAccount = destinationAccountEntity!;

    string sourceCurrency = payload.SourceCurrency.Trim().ToUpperInvariant();
    string destinationCurrency = payload.DestinationCurrency.Trim().ToUpperInvariant();

    decimal? fxRate = payload.FxRate;
    if (
      fxRate is null
      && !sourceCurrency.Equals(destinationCurrency, StringComparison.OrdinalIgnoreCase)
    )
    {
      fxRate = payload.SourceAmount == 0 ? null : payload.DestinationAmount / payload.SourceAmount;
    }

    if (fxRate is < 0)
    {
      errors.Add(
        new ValidationError("activity.fx_invalid", "FX rate must be positive when provided.")
      );
      return Result<IReadOnlyList<ActivityDetailsDto>>.Failure(errors);
    }

    await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(
      cancellationToken
    );

    var transferGroupId = Guid.NewGuid();
    DateTimeOffset timestamp = DateTimeOffset.UtcNow;

    Activity sourceActivity = new()
    {
      UserId = userId,
      AccountId = sourceAccount.Id,
      Type = ActivityType.TransferFrom,
      Amount = -Math.Abs(payload.SourceAmount),
      Currency = sourceCurrency,
      FxRate = fxRate,
      SourceCurrency = sourceCurrency,
      Description = payload.Description?.Trim(),
      TransferGroupId = transferGroupId,
      OccurredOn = payload.OccurredOn,
      CreatedUtc = timestamp,
      UpdatedUtc = timestamp,
    };

    Activity destinationActivity = new()
    {
      UserId = userId,
      AccountId = destinationAccount.Id,
      Type = ActivityType.TransferTo,
      Amount = Math.Abs(payload.DestinationAmount),
      Currency = destinationCurrency,
      FxRate = fxRate,
      SourceCurrency = destinationCurrency,
      Description = payload.Description?.Trim(),
      TransferGroupId = transferGroupId,
      OccurredOn = payload.OccurredOn,
      CreatedUtc = timestamp,
      UpdatedUtc = timestamp,
    };

    await context.Activities.AddRangeAsync(
      [sourceActivity, destinationActivity],
      cancellationToken
    );

    sourceAccount.UpdatedUtc = timestamp;
    destinationAccount.UpdatedUtc = timestamp;

    await context.SaveChangesAsync(cancellationToken);
    await transaction.CommitAsync(cancellationToken);

    IReadOnlyList<ActivityDetailsDto> dtos =
    [
      sourceActivity.ToDetailsDto(),
      destinationActivity.ToDetailsDto(),
    ];

    return Result<IReadOnlyList<ActivityDetailsDto>>.Success(dtos);
  }

  public async Task<Result<ActivityDetailsDto>> GetActivityAsync(
    Guid userId,
    Guid activityId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Activity? entity = await context
      .Activities.AsNoTracking()
      .Include(activity => activity.Asset)
      .FirstOrDefaultAsync(
        activity => activity.Id == activityId && activity.UserId == userId,
        cancellationToken
      );

    if (entity is null)
    {
      return Result<ActivityDetailsDto>.Failure(
        new ValidationError("activity.not_found", "Activity was not found.")
      );
    }

    return Result<ActivityDetailsDto>.Success(entity.ToDetailsDto());
  }

  public async Task<Result<IReadOnlyList<ActivitySummaryDto>>> ListActivitiesAsync(
    Guid userId,
    Guid accountId,
    ActivityType? type,
    Guid? assetId,
    DateTimeOffset? occurredFrom,
    DateTimeOffset? occurredTo,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    IQueryable<Activity> query = context
      .Activities.AsNoTracking()
      .Where(activity => activity.AccountId == accountId && activity.UserId == userId);

    if (type.HasValue)
    {
      query = query.Where(activity => activity.Type == type);
    }

    if (assetId.HasValue)
    {
      query = query.Where(activity => activity.AssetId == assetId);
    }

    if (occurredFrom.HasValue)
    {
      query = query.Where(activity => activity.OccurredOn >= occurredFrom);
    }

    if (occurredTo.HasValue)
    {
      query = query.Where(activity => activity.OccurredOn <= occurredTo);
    }

    List<Activity> entities = await query
      .OrderByDescending(activity => activity.OccurredOn)
      .ThenByDescending(activity => activity.CreatedUtc)
      .ToListAsync(cancellationToken);

    IReadOnlyList<ActivitySummaryDto> results = entities
      .Select(activity => activity.ToSummaryDto())
      .ToList();

    return Result<IReadOnlyList<ActivitySummaryDto>>.Success(results);
  }

  private static ValidationError[] ValidateCommonAmounts(decimal amount, string currency)
  {
    List<ValidationError> errors = [];

    if (amount <= 0)
    {
      errors.Add(new ValidationError("activity.amount_invalid", "Amount must be positive."));
    }

    if (string.IsNullOrWhiteSpace(currency) || !IsIsoCurrency(currency))
    {
      errors.Add(
        new ValidationError("activity.currency_invalid", "Currency must be a 3-letter ISO code.")
      );
    }

    return [.. errors];
  }

  private static decimal ApplyCashSign(ActivityType type, decimal amount)
  {
    decimal absolute = Math.Abs(amount);
    return type switch
    {
      ActivityType.Withdrawal or ActivityType.Fee or ActivityType.Tax => -absolute,
      _ => absolute,
    };
  }

  private static decimal ApplyTradeSign(ActivityType type, decimal amount)
  {
    decimal absolute = Math.Abs(amount);
    return type switch
    {
      ActivityType.Buy => -absolute,
      _ => absolute,
    };
  }

  private static decimal ApplyTradeQuantity(ActivityType type, decimal quantity)
  {
    decimal absolute = Math.Abs(quantity);
    return type switch
    {
      ActivityType.Sell => -absolute,
      _ => absolute,
    };
  }

  private static bool IsIsoCurrency(string currency)
  {
    string trimmed = currency.Trim();
    if (trimmed.Length != 3)
    {
      return false;
    }

    foreach (char c in trimmed)
    {
      if (!char.IsLetter(c))
      {
        return false;
      }
    }

    return true;
  }

  private static async Task<(Account? Account, ValidationError? Error)> TryGetActiveAccountAsync(
    FolioDbContext context,
    Guid userId,
    Guid accountId,
    CancellationToken cancellationToken
  )
  {
    Account? account = await context.Accounts.FirstOrDefaultAsync(
      entity => entity.Id == accountId && entity.UserId == userId,
      cancellationToken
    );

    if (account is null)
    {
      return (null, new ValidationError("activity.account_not_found", "Account was not found."));
    }

    if (account.IsDeleted)
    {
      return (
        null,
        new ValidationError(
          "activity.account_archived",
          "Account is archived and cannot accept new activities."
        )
      );
    }

    return (account, null);
  }

  private static async Task<(Asset? Asset, ValidationError? Error)> TryGetActiveAssetAsync(
    FolioDbContext context,
    Guid userId,
    Guid assetId,
    CancellationToken cancellationToken
  )
  {
    Asset? asset = await context.Assets.FirstOrDefaultAsync(
      entity => entity.Id == assetId && entity.UserId == userId,
      cancellationToken
    );

    if (asset is null)
    {
      return (null, new ValidationError("activity.asset_not_found", "Asset was not found."));
    }

    if (asset.IsDeleted)
    {
      return (
        null,
        new ValidationError(
          "activity.asset_archived",
          "Asset is archived and cannot be used for trades."
        )
      );
    }

    return (asset, null);
  }
}
