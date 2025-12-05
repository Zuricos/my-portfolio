using Microsoft.EntityFrameworkCore;
using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Accounts;
using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Api.Application.Mappers;
using Zuricos.Folio.Data;
using Zuricos.Folio.Data.Enums;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Services;

/// <summary>
/// Default implementation of <see cref="IAccountService"/>.
/// </summary>
public sealed class AccountService : IAccountService
{
  private static readonly ActivityType[] TransferTypes =
  [
    ActivityType.TransferFrom,
    ActivityType.TransferTo,
  ];

  private readonly IDbContextFactory<FolioDbContext> _dbContextFactory;
  private readonly ILogger<AccountService> _logger;

  public AccountService(
    IDbContextFactory<FolioDbContext> dbContextFactory,
    ILogger<AccountService> logger
  )
  {
    _dbContextFactory = dbContextFactory;
    _logger = logger;
  }

  public async Task<Result<AccountDetailsDto>> CreateAccountAsync(
    Guid userId,
    AccountCreateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] validationErrors = ValidateAccountPayload(
      payload.Name,
      payload.DisplayCurrency
    );
    if (validationErrors.Length > 0)
    {
      return Result<AccountDetailsDto>.Failure(validationErrors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Portfolio? portfolio = await context.Portfolios.FirstOrDefaultAsync(
      p => p.Id == payload.PortfolioId && p.UserId == userId,
      cancellationToken
    );

    if (portfolio is null)
    {
      return Result<AccountDetailsDto>.Failure(
        new ValidationError("account.portfolio_not_found", "Portfolio was not found.")
      );
    }

    bool nameExists = await context.Accounts.AnyAsync(
      account =>
        account.PortfolioId == payload.PortfolioId
        && account.UserId == userId
        && account.Name.ToLower() == payload.Name.ToLower()
        && !account.IsDeleted,
      cancellationToken
    );

    if (nameExists)
    {
      return Result<AccountDetailsDto>.Failure(
        new ValidationError("account.duplicate", "An account with the same name already exists.")
      );
    }

    Account entity = new()
    {
      UserId = userId,
      PortfolioId = payload.PortfolioId,
      Name = payload.Name.Trim(),
      Type = payload.Type,
      DisplayCurrency = payload.DisplayCurrency.Trim().ToUpperInvariant(),
      Category = payload.Category?.Trim(),
      CreatedUtc = DateTimeOffset.UtcNow,
      UpdatedUtc = DateTimeOffset.UtcNow,
    };

    await context.Accounts.AddAsync(entity, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    AccountDetailsDto dto = await BuildAccountDetailsAsync(context, entity.Id, cancellationToken);
    return Result<AccountDetailsDto>.Success(dto);
  }

  public async Task<Result<AccountDetailsDto>> UpdateAccountAsync(
    Guid userId,
    Guid accountId,
    AccountUpdateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] validationErrors = ValidateAccountPayload(
      payload.Name,
      payload.DisplayCurrency
    );
    if (validationErrors.Length > 0)
    {
      return Result<AccountDetailsDto>.Failure(validationErrors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Account? entity = await context.Accounts.FirstOrDefaultAsync(
      account => account.Id == accountId && account.UserId == userId,
      cancellationToken
    );

    if (entity is null)
    {
      return Result<AccountDetailsDto>.Failure(
        new ValidationError("account.not_found", "Account was not found.")
      );
    }

    if (!string.Equals(entity.Name, payload.Name, StringComparison.OrdinalIgnoreCase))
    {
      bool nameExists = await context.Accounts.AnyAsync(
        account =>
          account.Id != accountId
          && account.PortfolioId == entity.PortfolioId
          && account.UserId == userId
          && account.Name.ToLower() == payload.Name.ToLower()
          && !account.IsDeleted,
        cancellationToken
      );

      if (nameExists)
      {
        return Result<AccountDetailsDto>.Failure(
          new ValidationError("account.duplicate", "An account with the same name already exists.")
        );
      }
    }

    entity.Name = payload.Name.Trim();
    entity.Type = payload.Type;
    entity.DisplayCurrency = payload.DisplayCurrency.Trim().ToUpperInvariant();
    entity.Category = payload.Category?.Trim();
    entity.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);

    AccountDetailsDto dto = await BuildAccountDetailsAsync(context, entity.Id, cancellationToken);
    return Result<AccountDetailsDto>.Success(dto);
  }

  public async Task<Result> ArchiveAccountAsync(
    Guid userId,
    Guid accountId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Account? entity = await context.Accounts.FirstOrDefaultAsync(
      account => account.Id == accountId && account.UserId == userId,
      cancellationToken
    );

    if (entity is null)
    {
      return Result.Failure(new ValidationError("account.not_found", "Account was not found."));
    }

    if (entity.IsDeleted)
    {
      return Result.Success();
    }

    bool hasOpenTransfers = await context.Activities.AnyAsync(
      activity =>
        activity.AccountId == accountId
        && !activity.IsDeleted
        && activity.TransferGroupId != null
        && TransferTypes.Contains(activity.Type),
      cancellationToken
    );

    if (hasOpenTransfers)
    {
      return Result.Failure(
        new ValidationError(
          "account.pending_transfer",
          "Account cannot be archived while it contains active transfer activities."
        )
      );
    }

    entity.IsDeleted = true;
    entity.DeletedUtc = DateTimeOffset.UtcNow;
    entity.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Account {AccountId} archived", accountId);

    return Result.Success();
  }

  public async Task<Result<AccountDetailsDto>> GetAccountAsync(
    Guid userId,
    Guid accountId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    bool exists = await context.Accounts.AnyAsync(
      account => account.Id == accountId && account.UserId == userId,
      cancellationToken
    );

    if (!exists)
    {
      return Result<AccountDetailsDto>.Failure(
        new ValidationError("account.not_found", "Account was not found.")
      );
    }

    AccountDetailsDto dto = await BuildAccountDetailsAsync(context, accountId, cancellationToken);
    return Result<AccountDetailsDto>.Success(dto);
  }

  public async Task<Result<IReadOnlyList<AccountSummaryDto>>> ListAccountsByPortfolioAsync(
    Guid userId,
    Guid portfolioId,
    AccountType? type,
    string? category,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    IQueryable<Account> query = context
      .Accounts.AsNoTracking()
      .Where(account => account.PortfolioId == portfolioId && account.UserId == userId);

    if (type.HasValue)
    {
      query = query.Where(account => account.Type == type);
    }

    if (!string.IsNullOrWhiteSpace(category))
    {
      string normalized = category.Trim().ToLower();
      query = query.Where(account => (account.Category ?? string.Empty).ToLower() == normalized);
    }

    List<Account> entities = await query
      .OrderBy(account => account.Name)
      .ToListAsync(cancellationToken);

    IReadOnlyList<AccountSummaryDto> results = entities
      .Select(account => account.ToSummaryDto())
      .ToList();

    return Result<IReadOnlyList<AccountSummaryDto>>.Success(results);
  }

  private static ValidationError[] ValidateAccountPayload(string name, string currency)
  {
    List<ValidationError> errors = [];

    if (string.IsNullOrWhiteSpace(name))
    {
      errors.Add(new ValidationError("account.name_required", "Name is required."));
    }

    if (string.IsNullOrWhiteSpace(currency))
    {
      errors.Add(new ValidationError("account.currency_required", "Display currency is required."));
    }
    else if (!IsIsoCurrency(currency))
    {
      errors.Add(
        new ValidationError(
          "account.currency_invalid",
          "Display currency must be a 3-letter ISO code."
        )
      );
    }

    return [.. errors];
  }

  private static bool IsIsoCurrency(string currency)
  {
    if (currency.Length != 3)
    {
      return false;
    }

    foreach (char c in currency)
    {
      if (!char.IsLetter(c))
      {
        return false;
      }
    }

    return true;
  }

  private static MoneyAmountDto CreateMoney(decimal amount, string currency) =>
    new(amount, currency);

  private static AccountAssetPositionDto CreatePosition(
    Guid assetId,
    string symbol,
    decimal quantity,
    decimal costBasis,
    string currency
  )
  {
    MoneyAmountDto cost = CreateMoney(costBasis, currency);
    return new AccountAssetPositionDto(assetId, symbol, quantity, cost, null);
  }

  private static decimal NormalizeAmount(Activity activity)
  {
    return activity.Type switch
    {
      ActivityType.Withdrawal
      or ActivityType.TransferFrom
      or ActivityType.Fee
      or ActivityType.Tax
      or ActivityType.Sell => -Math.Abs(activity.Amount),
      _ => activity.Amount,
    };
  }

  private static decimal NormalizeQuantity(Activity activity)
  {
    return activity.Type switch
    {
      ActivityType.Sell or ActivityType.TransferFrom => -Math.Abs(activity.Quantity),
      _ => activity.Quantity,
    };
  }

  private static async Task<AccountDetailsDto> BuildAccountDetailsAsync(
    FolioDbContext context,
    Guid accountId,
    CancellationToken cancellationToken
  )
  {
    Account account = await context
      .Accounts.AsNoTracking()
      .FirstAsync(account => account.Id == accountId, cancellationToken);

    List<Activity> activities = await context
      .Activities.AsNoTracking()
      .Where(activity => activity.AccountId == accountId && !activity.IsDeleted)
      .ToListAsync(cancellationToken);

    decimal balance = 0m;
    foreach (Activity activity in activities)
    {
      balance += NormalizeAmount(activity);
    }

    MoneyAmountDto bookBalance = CreateMoney(balance, account.DisplayCurrency);

    var assetIds = activities
      .Where(activity => activity.AssetId.HasValue)
      .Select(activity => activity.AssetId!.Value)
      .Distinct()
      .ToList();

    Dictionary<Guid, string> assetSymbols;
    if (assetIds.Count == 0)
    {
      assetSymbols = [];
    }
    else
    {
      assetSymbols = await context
        .Assets.AsNoTracking()
        .Where(asset => assetIds.Contains(asset.Id))
        .ToDictionaryAsync(asset => asset.Id, asset => asset.Symbol, cancellationToken);
    }

    IReadOnlyList<AccountAssetPositionDto> positions = activities
      .Where(activity => activity.AssetId.HasValue)
      .GroupBy(activity => activity.AssetId!.Value)
      .Select(group =>
      {
        Guid assetId = group.Key;
        decimal quantity = group.Sum(NormalizeQuantity);
        decimal costBasis = group.Sum(NormalizeAmount);
        string symbol = assetSymbols.TryGetValue(assetId, out string? value) ? value : string.Empty;

        return CreatePosition(assetId, symbol, quantity, costBasis, account.DisplayCurrency);
      })
      .ToList();

    return account.ToDetailsDto(bookBalance, positions);
  }
}
