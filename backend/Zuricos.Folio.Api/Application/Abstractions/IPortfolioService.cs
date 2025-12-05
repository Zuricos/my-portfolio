using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Portfolios;

namespace Zuricos.Folio.Api.Application.Abstractions;

/// <summary>
/// Service boundary for portfolio workflows.
/// </summary>
public interface IPortfolioService
{
  Task<Result<PortfolioDetailsDto>> CreatePortfolioAsync(
    Guid userId,
    PortfolioCreateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result<PortfolioDetailsDto>> UpdatePortfolioAsync(
    Guid userId,
    Guid portfolioId,
    PortfolioUpdateCommand payload,
    CancellationToken cancellationToken = default
  );

  Task<Result> ArchivePortfolioAsync(
    Guid userId,
    Guid portfolioId,
    CancellationToken cancellationToken = default
  );

  Task<Result<PortfolioDetailsDto>> GetPortfolioAsync(
    Guid userId,
    Guid portfolioId,
    CancellationToken cancellationToken = default
  );

  Task<Result<IReadOnlyList<PortfolioSummaryDto>>> ListPortfoliosAsync(
    Guid userId,
    string? search,
    CancellationToken cancellationToken = default
  );
}
