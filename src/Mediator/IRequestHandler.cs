namespace Catalog.Mediator;

// интерфейс-обработчик
public interface IRequestHandler<in TRequest, TResponse>
		where TRequest : IRequest<TResponse>
{
	// обработчик запроса
	Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
