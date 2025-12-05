namespace Zuricos.Folio.Api.Application.Common;

/// <summary>
/// Canonical result type for application services. Encapsulates success state and validation errors.
/// </summary>
public class Result
{
  public bool IsSuccess { get; }

  public bool IsFailure => !IsSuccess;

  public IReadOnlyList<ValidationError> Errors { get; }

  protected Result(bool isSuccess, IReadOnlyList<ValidationError> errors)
  {
    IsSuccess = isSuccess;
    Errors = errors;
  }

  public static Result Success() => new(true, Array.Empty<ValidationError>());

  public static Result Failure(params ValidationError[] errors) => new(false, errors);

  public static Result Failure(IEnumerable<ValidationError> errors) => new(false, errors.ToArray());
}

/// <summary>
/// Strongly-typed result with payload.
/// </summary>
public sealed class Result<T> : Result
{
  public T? Value { get; }

  private Result(bool isSuccess, T? value, IReadOnlyList<ValidationError> errors)
    : base(isSuccess, errors) => Value = value;

  public static Result<T> Success(T value) => new(true, value, Array.Empty<ValidationError>());

  public static new Result<T> Failure(params ValidationError[] errors) =>
    new(false, default, errors);

  public static new Result<T> Failure(IEnumerable<ValidationError> errors) =>
    new(false, default, errors.ToArray());
}

/// <summary>
/// Simple validation error descriptor used across service boundaries.
/// </summary>
public sealed record ValidationError(string Code, string Message, string? Field = null);
