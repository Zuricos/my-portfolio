using Microsoft.AspNetCore.Mvc;

namespace Zuricos.Folio.Api.Application.Common;

/// <summary>
/// Helpers to translate application-level <see cref="Result"/> objects into HTTP-friendly responses.
/// </summary>
public static class ResultHttpExtensions
{
  private const string NotFoundSuffix = ".not_found";

  public static IResult ToOk<T>(this Result<T> result) => result.Match(TypedResults.Ok);

  public static IResult ToOk(this Result result) =>
    result.IsSuccess ? TypedResults.Ok() : CreateProblem(result.Errors);

  public static IResult ToCreated<T>(
    this Result<T> result,
    string routeName,
    Func<T, object> routeValuesFactory
  ) =>
    result.Match(value => TypedResults.CreatedAtRoute(value, routeName, routeValuesFactory(value)));

  public static IResult ToNoContent(this Result result) =>
    result.IsSuccess ? TypedResults.NoContent() : CreateProblem(result.Errors);

  private static IResult Match<T>(this Result<T> result, Func<T, IResult> onSuccess)
  {
    if (result.IsSuccess)
    {
      return onSuccess(result.Value!);
    }

    return CreateProblem(result.Errors);
  }

  private static IResult CreateProblem(IReadOnlyList<ValidationError> errors)
  {
    if (errors.Count == 0)
    {
      return TypedResults.Problem(
        statusCode: StatusCodes.Status500InternalServerError,
        title: "Unknown error"
      );
    }

    if (errors.All(IsNotFound))
    {
      ProblemDetails notFound = new()
      {
        Title = "Resource not found",
        Status = StatusCodes.Status404NotFound,
        Detail = errors[0].Message,
      };
      notFound.Extensions["errors"] = errors
        .Select(error => new
        {
          error.Code,
          error.Message,
          error.Field,
        })
        .ToArray();
      return TypedResults.Problem(notFound);
    }

    IDictionary<string, string[]> errorDictionary = BuildValidationDictionary(errors);
    ProblemDetails validationProblem = new()
    {
      Status = StatusCodes.Status422UnprocessableEntity,
      Title = "Validation failed",
    };
    validationProblem.Extensions["errors"] = errorDictionary;
    return TypedResults.Problem(validationProblem);
  }

  private static IDictionary<string, string[]> BuildValidationDictionary(
    IEnumerable<ValidationError> errors
  )
  {
    return errors
      .GroupBy(error => string.IsNullOrWhiteSpace(error.Field) ? error.Code : error.Field!)
      .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
  }

  private static bool IsNotFound(ValidationError error) =>
    error.Code.EndsWith(NotFoundSuffix, StringComparison.OrdinalIgnoreCase);
}
