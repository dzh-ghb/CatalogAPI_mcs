namespace Catalog.Api.Exceptions.Handler;

// обработчик исключений
public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		logger.LogError(
				"Ошибка: {exceptionMessage}, время: {time}",
				exception.Message,
				DateTime.UtcNow
		// DateTime.Now
		);

		(string Detail, string Title, int StatusCode) details = exception switch
		{
			BookNotFoundException => (
					exception.Message,
					exception.GetType().Name,
					httpContext.Response.StatusCode = StatusCodes.Status404NotFound
			),
			BookDomainException => (
					exception.Message,
					exception.GetType().Name,
					httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
			),
			ValidationException => (
					exception.Message,
					exception.GetType().Name,
					httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
			),
			_ => (
					exception.Message,
					exception.GetType().Name,
					httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
			)
		};

		var problems = new ProblemDetails
		{
			Detail = details.Detail,
			Title = details.Title,
			Status = details.StatusCode,
			Instance = httpContext.Request.Path
		};

		if (exception is ValidationException validationException)
		{
			problems.Extensions.Add("errors", validationException.Errors);
		}

		await httpContext.Response.WriteAsJsonAsync(
				problems,
				cancellationToken: cancellationToken
		);

		return true;
	}
}