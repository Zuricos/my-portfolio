using Zuricos.Folio.Api.Application.Abstractions;
using Zuricos.Folio.Api.Application.Common;
using Zuricos.Folio.Api.Application.Contracts.Accounts;
using Zuricos.Folio.Data.Const;
using Zuricos.Folio.Data.Enums;

namespace Zuricos.Folio.Api.Endpoints;

public static class AccountEndpoints
{
  private const string GetAccountRouteName = "Accounts_GetById";

  public static RouteGroupBuilder MapAccountEndpoints(this RouteGroupBuilder apiGroup)
  {
    RouteGroupBuilder group = apiGroup.MapGroup("accounts").WithTags("Accounts");

    group
      .MapPost(
        "/",
        async (
          IAccountService service,
          AccountCreateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<AccountDetailsDto> result = await service.CreateAccountAsync(
            userId,
            command,
            cancellationToken
          );
          return result.ToCreated(GetAccountRouteName, dto => new { accountId = dto.Id });
        }
      )
      .WithName("Accounts_Create")
      .WithOpenApi();

    group
      .MapGet(
        "/{accountId:guid}",
        async (IAccountService service, Guid accountId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<AccountDetailsDto> result = await service.GetAccountAsync(
            userId,
            accountId,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName(GetAccountRouteName)
      .WithOpenApi();

    group
      .MapPut(
        "/{accountId:guid}",
        async (
          IAccountService service,
          Guid accountId,
          AccountUpdateCommand command,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<AccountDetailsDto> result = await service.UpdateAccountAsync(
            userId,
            accountId,
            command,
            cancellationToken
          );
          return result.ToOk();
        }
      )
      .WithName("Accounts_Update")
      .WithOpenApi();

    group
      .MapDelete(
        "/{accountId:guid}",
        async (IAccountService service, Guid accountId, CancellationToken cancellationToken) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result result = await service.ArchiveAccountAsync(userId, accountId, cancellationToken);
          return result.ToNoContent();
        }
      )
      .WithName("Accounts_Delete")
      .WithOpenApi();

    apiGroup
      .MapGet(
        "portfolios/{portfolioId:guid}/accounts",
        async (
          IAccountService service,
          Guid portfolioId,
          AccountType? type,
          string? category,
          CancellationToken cancellationToken
        ) =>
        {
          Guid userId = ConstValues.DefaultUserId;
          Result<IReadOnlyList<AccountSummaryDto>> result =
            await service.ListAccountsByPortfolioAsync(
              userId,
              portfolioId,
              type,
              category,
              cancellationToken
            );
          return result.ToOk();
        }
      )
      .WithTags("Accounts")
      .WithName("Accounts_ListByPortfolio")
      .WithOpenApi();

    return apiGroup;
  }
}
