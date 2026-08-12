namespace Catalog.Mediator;

// класс для поиска нужного обработчика в DI-контейнере и его вызова
public class Sender(IServiceProvider services) : ISender
{
	private static readonly ConcurrentDictionary<Type, Object> requestHandlers = new();

	// метод для сопоставления запроса и нужного хендлера из DI-контейнера
	public Task<TResponse> Send<TResponse>(
		IRequest<TResponse> request,
		CancellationToken cancellationToken = default)
	{
		var wrapper = (RequestHandlerWrapper<TResponse>)(requestHandlers.GetOrAdd(
			request.GetType(), // тип объекта через рантайм
			requestType => Activator.CreateInstance(
				typeof(RequestHandlerWrapper<,>)
					.MakeGenericType(requestType, typeof(TResponse))
			)!)
		);

		return wrapper.Handle(request, services, cancellationToken);
	}
}

// абстракция с обобщением по ответу
public abstract class RequestHandlerWrapper<TResponse>
{
	public abstract Task<TResponse> Handle(
		IRequest<TResponse> request,
		IServiceProvider services,
		CancellationToken cancellationToken = default);
}

// реализация с обобщением и по запросу, и по ответу
public class RequestHandlerWrapper<TRequest, TResponse>
	: RequestHandlerWrapper<TResponse>
	where TRequest : IRequest<TResponse>
{
	public override Task<TResponse> Handle(
		IRequest<TResponse> request,
		IServiceProvider services,
		CancellationToken cancellationToken = default)
	{
		var handler = services.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

		return handler.Handle((TRequest)request, cancellationToken);
	}
}