namespace Catalog.Api.Common;

public enum ErrorType
{
	NotFound,
	Validation,
	Conflict
}

public record Error(ErrorType TypeError, string Message);

public class Result<T>
{
	public bool IsSuccess { get; }
	public T? Value { get; }
	public Error? Error { get; }

	// доступ только через фабричные методы (нельзя собрать некорректный результат)
	private Result(bool isSuccess, T? value, Error? error)
	{
		this.IsSuccess = isSuccess;
		this.Value = value;
		this.Error = error;
	}

	public static Result<T> Success(T value) => new(true, value, null);

	public static Result<T> Failure(ErrorType type, string message) => new(false, default, new Error(type, message));

	public static Result<T> NotFound(string message) => Failure(ErrorType.NotFound, message);

	// метод для получения результата (вызывает подходящую функцию)
	public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure) =>
		IsSuccess ? onSuccess(Value!) : onFailure(Error!);
}