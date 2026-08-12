namespace Catalog.Mediator;

// промежуточное звено конвейера
public interface IPipelineBehavior<in TRequest, TResponse>
	where TRequest : notnull
{
	Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken);
}