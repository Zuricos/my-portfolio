using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Accounts;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Application.Abstractions;

/// <summary>
/// Service boundary for account management workflows.
/// </summary>
public interface IAccountService
{
  Task<Result<AccountDetailsDto>> CreateAccountAsync(
    Guid userId,
    AccountCreateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result<AccountDetailsDto>> UpdateAccountAsync(
    Guid userId,
    Guid accountId,
    AccountUpdateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result> ArchiveAccountAsync(
    Guid userId,
    Guid accountId,
    CancellationToken cancellationToken = default
  );

  Task<Result<AccountDetailsDto>> GetAccountAsync(
    Guid userId,
    Guid accountId,
    CancellationToken cancellationToken = default
  );

  Task<Result<IReadOnlyList<AccountSummaryDto>>> ListAccountsByPortfolioAsync(
    Guid userId,
    Guid portfolioId,
    AccountType? type,
    string? category,
    CancellationToken cancellationToken = default
  );
}
