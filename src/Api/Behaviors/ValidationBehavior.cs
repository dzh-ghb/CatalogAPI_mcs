namespace Catalog.Api.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
		: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse> // upd.: только обработка команд, для валидации и запросов заменить ICommand на IRequest
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var context = new ValidationContext<TRequest>(request);

		var validationResult = await Task.WhenAll(
				validators.Select(v => v.ValidateAsync(context, cancellationToken))
		);

		var failures = validationResult
				.Where(r => r.Errors.Any())
				.SelectMany(r => r.Errors)
				.ToList();

		if (failures.Any())
		{
			throw new ValidationException(failures);
		}

		return await next();
	}
}