using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Zuricos.Folio.Api.Endpoints;

/// <summary>
/// Central registry for the API surface exposed via minimal APIs.
/// </summary>
public static class ApiEndpointExtensions
{
  public static void MapApiEndpoints(this IEndpointRouteBuilder endpoints)
  {
    RouteGroupBuilder apiGroup = endpoints.MapGroup("/api").WithOpenApi();

    apiGroup.MapPortfolioEndpoints();
    apiGroup.MapAccountEndpoints();
    apiGroup.MapActivityEndpoints();
    apiGroup.MapAssetEndpoints();
  }
}
