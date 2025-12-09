using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Portfolios;
using Zuricos.Folio.Data.Const;

namespace Zuricos.Folio.Api.Endpoints;

public static class PortfolioEndpoints
{
  private const string GetPortfolioRouteName = "Portfolios_GetById";

  public static RouteGroupBuilder MapPortfolioEndpoints(this RouteGroupBuilder apiGroup)
  {
    RouteGroupBuilder group = apiGroup.MapGroup("portfolios").WithTags("Portfolios");

    group
      .MapPost(
        "/",
        async (
          IPortfolioService service,
          PortfolioCreateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<PortfolioDetailsDto> result = await service.CreatePortfolioAsync(
            userId,
            command,
            cancellationToken
          );
          return result.ToCreated(GetPortfolioRouteName, dto => new { portfolioId = dto.Id });
        }
      )
      .WithName("Portfolios_Create")
      .WithOpenApi();

    group
      .MapGet(
        "/{portfolioId:guid}",
        async (IPortfolioService service, Guid portfolioId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<PortfolioDetailsDto> result = await service.GetPortfolioAsync(
            userId,
            portfolioId,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName(GetPortfolioRouteName)
      .WithOpenApi();

    group
      .MapGet(
        "/",
        async (IPortfolioService service, string? search, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<IReadOnlyList<PortfolioSummaryDto>> result = await service.ListPortfoliosAsync(
            userId,
            search,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName("Portfolios_List")
      .WithOpenApi();

    group
      .MapPut(
        "/{portfolioId:guid}",
        async (
          IPortfolioService service,
          Guid portfolioId,
          PortfolioUpdateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<PortfolioDetailsDto> result = await service.UpdatePortfolioAsync(
            userId,
            portfolioId,
            command,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName("Portfolios_Update")
      .WithOpenApi();

    group
      .MapDelete(
        "/{portfolioId:guid}",
        async (IPortfolioService service, Guid portfolioId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result result = await service.ArchivePortfolioAsync(
            userId,
            portfolioId,
            cancellationToken
          );
          return result.ToNoContent();
        }
      )
      .WithName("Portfolios_Delete")
      .WithOpenApi();

    return apiGroup;
  }
}
