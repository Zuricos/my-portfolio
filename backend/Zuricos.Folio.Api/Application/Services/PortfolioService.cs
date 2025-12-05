using Microsoft.EntityFrameworkCore;
using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Portfolios;
using Zuricos.Folio.Api.Application.Mappers;
using Zuricos.Folio.Data;
using Zuricos.Folio.Data.Models;

namespace Zuricos.Folio.Api.Application.Services;

/// <summary>
/// Default implementation of <see cref="IPortfolioService"/>.
/// </summary>
public sealed class PortfolioService : IPortfolioService
{
  private readonly IDbContextFactory<FolioDbContext> _dbContextFactory;
  private readonly ILogger<PortfolioService> _logger;

  public PortfolioService(
    IDbContextFactory<FolioDbContext> dbContextFactory,
    ILogger<PortfolioService> logger
  )
  {
    _dbContextFactory = dbContextFactory;
    _logger = logger;
  }

  public async Task<Result<PortfolioDetailsDto>> CreatePortfolioAsync(
    Guid userId,
    PortfolioCreateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] validationErrors = ValidatePortfolioPayload(
      payload.Name,
      payload.DisplayCurrency
    );
    if (validationErrors.Length > 0)
    {
      return Result<PortfolioDetailsDto>.Failure(validationErrors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    bool nameExists = await context.Portfolios.AnyAsync(
      portfolio =>
        portfolio.UserId == userId
        && portfolio.Name.ToLower() == payload.Name.ToLower()
        && !portfolio.IsDeleted,
      cancellationToken
    );

    if (nameExists)
    {
      return Result<PortfolioDetailsDto>.Failure(
        new ValidationError("portfolio.duplicate", "A portfolio with the same name already exists.")
      );
    }

    Portfolio entity = new()
    {
      UserId = userId,
      Name = payload.Name.Trim(),
      DisplayCurrency = payload.DisplayCurrency.Trim().ToUpperInvariant(),
      Institution = payload.Institution?.Trim(),
      Notes = payload.Notes?.Trim(),
      CreatedUtc = DateTimeOffset.UtcNow,
      UpdatedUtc = DateTimeOffset.UtcNow,
    };

    await context.Portfolios.AddAsync(entity, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    await context
      .Entry(entity)
      .Collection(portfolio => portfolio.Accounts)
      .LoadAsync(cancellationToken);

    return Result<PortfolioDetailsDto>.Success(entity.ToDetailsDto());
  }

  public async Task<Result<PortfolioDetailsDto>> UpdatePortfolioAsync(
    Guid userId,
    Guid portfolioId,
    PortfolioUpdateCommand payload,
    CancellationToken cancellationToken = default
  )
  {
    ValidationError[] validationErrors = ValidatePortfolioPayload(
      payload.Name,
      payload.DisplayCurrency
    );
    if (validationErrors.Length > 0)
    {
      return Result<PortfolioDetailsDto>.Failure(validationErrors);
    }

    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Portfolio? entity = await context
      .Portfolios.Include(portfolio => portfolio.Accounts)
      .FirstOrDefaultAsync(
        portfolio => portfolio.Id == portfolioId && portfolio.UserId == userId,
        cancellationToken
      );

    if (entity is null)
    {
      return Result<PortfolioDetailsDto>.Failure(
        new ValidationError("portfolio.not_found", "Portfolio was not found.")
      );
    }

    if (!string.Equals(entity.Name, payload.Name, StringComparison.OrdinalIgnoreCase))
    {
      bool nameExists = await context.Portfolios.AnyAsync(
        portfolio =>
          portfolio.Id != portfolioId
          && portfolio.UserId == userId
          && portfolio.Name.ToLower() == payload.Name.ToLower()
          && !portfolio.IsDeleted,
        cancellationToken
      );

      if (nameExists)
      {
        return Result<PortfolioDetailsDto>.Failure(
          new ValidationError(
            "portfolio.duplicate",
            "A portfolio with the same name already exists."
          )
        );
      }
    }

    entity.Name = payload.Name.Trim();
    entity.DisplayCurrency = payload.DisplayCurrency.Trim().ToUpperInvariant();
    entity.Institution = payload.Institution?.Trim();
    entity.Notes = payload.Notes?.Trim();
    entity.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);

    return Result<PortfolioDetailsDto>.Success(entity.ToDetailsDto());
  }

  public async Task<Result> ArchivePortfolioAsync(
    Guid userId,
    Guid portfolioId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Portfolio? entity = await context
      .Portfolios.Include(portfolio => portfolio.Accounts)
      .FirstOrDefaultAsync(
        portfolio => portfolio.Id == portfolioId && portfolio.UserId == userId,
        cancellationToken
      );

    if (entity is null)
    {
      return Result.Failure(new ValidationError("portfolio.not_found", "Portfolio was not found."));
    }

    bool activeAccounts = entity.Accounts.Any(account => !account.IsDeleted);
    if (activeAccounts)
    {
      return Result.Failure(
        new ValidationError(
          "portfolio.active_accounts",
          "Portfolio cannot be archived while it still has active accounts."
        )
      );
    }

    if (entity.IsDeleted)
    {
      return Result.Success();
    }

    entity.IsDeleted = true;
    entity.DeletedUtc = DateTimeOffset.UtcNow;
    entity.UpdatedUtc = DateTimeOffset.UtcNow;

    await context.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Portfolio {PortfolioId} archived", portfolioId);

    return Result.Success();
  }

  public async Task<Result<PortfolioDetailsDto>> GetPortfolioAsync(
    Guid userId,
    Guid portfolioId,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    Portfolio? entity = await context
      .Portfolios.Include(portfolio => portfolio.Accounts)
      .FirstOrDefaultAsync(
        portfolio => portfolio.Id == portfolioId && portfolio.UserId == userId,
        cancellationToken
      );

    if (entity is null)
    {
      return Result<PortfolioDetailsDto>.Failure(
        new ValidationError("portfolio.not_found", "Portfolio was not found.")
      );
    }

    return Result<PortfolioDetailsDto>.Success(entity.ToDetailsDto());
  }

  public async Task<Result<IReadOnlyList<PortfolioSummaryDto>>> ListPortfoliosAsync(
    Guid userId,
    string? search,
    CancellationToken cancellationToken = default
  )
  {
    await using FolioDbContext context = await _dbContextFactory.CreateDbContextAsync(
      cancellationToken
    );

    IQueryable<Portfolio> query = context
      .Portfolios.AsNoTracking()
      .Include(portfolio => portfolio.Accounts)
      .Where(portfolio => portfolio.UserId == userId);

    if (!string.IsNullOrWhiteSpace(search))
    {
      string term = search.Trim().ToLower();
      query = query.Where(portfolio =>
        portfolio.Name.ToLower().Contains(term)
        || (portfolio.Institution ?? string.Empty).ToLower().Contains(term)
      );
    }

    List<Portfolio> entities = await query
      .OrderBy(portfolio => portfolio.Name)
      .ToListAsync(cancellationToken);

    IReadOnlyList<PortfolioSummaryDto> results = entities
      .Select(portfolio => portfolio.ToSummaryDto())
      .ToList();

    return Result<IReadOnlyList<PortfolioSummaryDto>>.Success(results);
  }

  private static ValidationError[] ValidatePortfolioPayload(string name, string currency)
  {
    List<ValidationError> errors = [];

    if (string.IsNullOrWhiteSpace(name))
    {
      errors.Add(new ValidationError("portfolio.name_required", "Name is required."));
    }

    if (string.IsNullOrWhiteSpace(currency))
    {
      errors.Add(
        new ValidationError("portfolio.currency_required", "Display currency is required.")
      );
    }
    else if (!IsIsoCurrency(currency))
    {
      errors.Add(
        new ValidationError(
          "portfolio.currency_invalid",
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
}
