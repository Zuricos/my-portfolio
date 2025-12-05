using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Assets;
using Zuricos.Folio.Api.Application.Contracts.Shared;
using Zuricos.Folio.Api.Application.Mappers;
using Zuricos.Folio.Data;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Services;

/// <summary>
/// Default implementation of <see cref="IAssetCatalogService"/>.
/// </summary>
public sealed class AssetCatalogService : IAssetCatalogService
{
  private readonly IDbContextFactory<FolioDbContext> _dbContextFactory;

  public AssetCatalogService(IDbContextFactory<FolioDbContext> dbContextFactory)
  {
    _dbContextFactory = dbContextFactory;
  }

  public async Task<Result<AssetDetailsDto>> CreateAssetAsync(
    Guid userId,
    AssetCreateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] validationErrors = ValidateAssetPayload(
      payload.Isin,
      payload.Name,
      payload.Symbol,
      payload.Currency
    );
    if (validationErrors.Length > 0)
    {
      return Result<AssetDetailsDto>.Failure(validationErrors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );
    await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(
      cancellationToken
    );

    string symbolUpper = payload.Symbol.Trim().ToUpperInvariant();
    bool symbolExists = await context.Assets.AnyAsync(
      asset =>
        asset.UserId == userId
        && asset.Symbol.ToLower() == symbolUpper.ToLower()
        && !asset.IsDeleted,
      cancellationToken
    );

    if (symbolExists)
    {
      return Result<AssetDetailsDto>.Failure(
        new ValidationError(
          "asset.duplicate_symbol",
          "An asset with the same symbol already exists."
        )
      );
    }

    Asset entity = new()
    {
      UserId = userId,
      Isin = payload.Isin.Trim().ToUpperInvariant(),
      Name = payload.Name.Trim(),
      Symbol = symbolUpper,
      Currency = payload.Currency.Trim().ToUpperInvariant(),
      DataSource = payload.DataSource,
      CreatedUtc = DateTimeOffset.UtcNow,
      UpdatedUtc = DateTimeOffset.UtcNow,
    };

    await context.Assets.AddAsync(entity, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);
    await transaction.CommitAsync(cancellationToken);

    AssetDetailsDto dto = entity.ToDetailsDto(null);
    return Result<AssetDetailsDto>.Success(dto);
  }

  public async Task<Result<AssetDetailsDto>> UpdateAssetAsync(
    Guid userId,
    Guid assetId,
    AssetUpdateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] validationErrors = ValidateAssetUpdatePayload(payload.Name, payload.Currency);
    if (validationErrors.Length > 0)
    {
      return Result<AssetDetailsDto>.Failure(validationErrors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Asset? entity = await context.Assets.FirstOrDefaultAsync(
      asset => asset.Id == assetId && asset.UserId == userId,
      cancellationToken
    );

    if (entity is null)
    {
      return Result<AssetDetailsDto>.Failure(
        new ValidationError("asset.not_found", "Asset was not found.")
      );
    }

    entity.Name = payload.Name.Trim();
    entity.Currency = payload.Currency.Trim().ToUpperInvariant();
    entity.DataSource = payload.DataSource;
    entity.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);

    AssetPriceSnapshotDto? latestPrice = await GetLatestPriceAsync(
      context,
      entity.Id,
      entity.Currency,
      cancellationToken
    );
    AssetDetailsDto dto = entity.ToDetailsDto(latestPrice);
    return Result<AssetDetailsDto>.Success(dto);
  }

  public async Task<Result> ArchiveAssetAsync(
    Guid userId,
    Guid assetId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Asset? entity = await context.Assets.FirstOrDefaultAsync(
      asset => asset.Id == assetId && asset.UserId == userId,
      cancellationToken
    );

    if (entity is null)
    {
      return Result.Failure(new ValidationError("asset.not_found", "Asset was not found."));
    }

    if (entity.IsDeleted)
    {
      return Result.Success();
    }

    entity.IsDeleted = true;
    entity.DeletedUtc = DateTimeOffset.UtcNow;
    entity.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);
    return Result.Success();
  }

  public async Task<Result<AssetDetailsDto>> GetAssetAsync(
    Guid userId,
    Guid assetId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Asset? entity = await context
      .Assets.AsNoTracking()
      .FirstOrDefaultAsync(
        asset => asset.Id == assetId && asset.UserId == userId,
        cancellationToken
      );

    if (entity is null)
    {
      return Result<AssetDetailsDto>.Failure(
        new ValidationError("asset.not_found", "Asset was not found.")
      );
    }

    AssetPriceSnapshotDto? latestPrice = await GetLatestPriceAsync(
      context,
      entity.Id,
      entity.Currency,
      cancellationToken
    );
    AssetDetailsDto dto = entity.ToDetailsDto(latestPrice);
    return Result<AssetDetailsDto>.Success(dto);
  }

  public async Task<Result<IReadOnlyList<AssetSummaryDto>>> ListAssetsAsync(
    Guid userId,
    string? search,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    IQueryable<Asset> query = context.Assets.AsNoTracking().Where(asset => asset.UserId == userId);

    if (!string.IsNullOrWhiteSpace(search))
    {
      string term = search.Trim().ToLower();
      query = query.Where(asset =>
        asset.Symbol.ToLower().Contains(term)
        || asset.Name.ToLower().Contains(term)
        || asset.Currency.ToLower().Contains(term)
      );
    }

    List<Asset> entities = await query
      .OrderBy(asset => asset.Symbol)
      .ThenBy(asset => asset.Name)
      .ToListAsync(cancellationToken);

    IReadOnlyList<AssetSummaryDto> dtos = entities.Select(asset => asset.ToSummaryDto()).ToList();

    return Result<IReadOnlyList<AssetSummaryDto>>.Success(dtos);
  }

  public async Task<Result> EnsureSeedAssetsAsync(
    Guid userId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    // Placeholder for future seeding logic (e.g., ensuring default cash asset entries).
    // Currently, simply ensures connection and returns success.
    _ = userId;

    return Result.Success();
  }

  private static ValidationError[] ValidateAssetPayload(
    string isin,
    string name,
    string symbol,
    string currency
  )
  {
    List<ValidationError> errors = [];

    if (string.IsNullOrWhiteSpace(isin) || isin.Trim().Length != 12)
    {
      errors.Add(new ValidationError("asset.isin_invalid", "ISIN must be 12 characters."));
    }

    if (string.IsNullOrWhiteSpace(name))
    {
      errors.Add(new ValidationError("asset.name_required", "Name is required."));
    }

    if (string.IsNullOrWhiteSpace(symbol))
    {
      errors.Add(new ValidationError("asset.symbol_required", "Symbol is required."));
    }

    errors.AddRange(ValidateAssetCurrency(currency));
    return [.. errors];
  }

  private static ValidationError[] ValidateAssetUpdatePayload(string name, string currency)
  {
    List<ValidationError> errors = [];

    if (string.IsNullOrWhiteSpace(name))
    {
      errors.Add(new ValidationError("asset.name_required", "Name is required."));
    }

    errors.AddRange(ValidateAssetCurrency(currency));
    return [.. errors];
  }

  private static ValidationError[] ValidateAssetCurrency(string currency)
  {
    if (string.IsNullOrWhiteSpace(currency))
    {
      return [new ValidationError("asset.currency_required", "Currency is required.")];
    }

    string trimmed = currency.Trim();
    if (trimmed.Length != 3 || trimmed.Any(c => !char.IsLetter(c)))
    {
      return
      [
        new ValidationError("asset.currency_invalid", "Currency must be a 3-letter ISO code."),
      ];
    }

    return Array.Empty<ValidationError>();
  }

  private static async Task<AssetPriceSnapshotDto?> GetLatestPriceAsync(
    FolioDbContext context,
    Guid assetId,
    string currency,
    CancellationToken cancellationToken
  )
  {
    AssetHistory? history = await context
      .AssetHistories.AsNoTracking()
      .Where(history => history.AssetId == assetId && !history.IsDeleted)
      .OrderByDescending(history => history.Date)
      .ThenByDescending(history => history.UpdatedUtc)
      .FirstOrDefaultAsync(cancellationToken);

    if (history is null)
    {
      return null;
    }

    var asOf = history.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
    return new AssetPriceSnapshotDto(
      new MoneyAmountDto(history.Close, currency.Trim().ToUpperInvariant()),
      new DateTimeOffset(asOf),
      "historical"
    );
  }
}
