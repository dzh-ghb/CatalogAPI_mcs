namespace Catalog.Mediator;

// точка отправки (через ISender эндпоинты отправляют команды и запросы)
public interface ISender
{
	Task<TResponse> Send<TResponse>(
		IRequest<TResponse> request,
		CancellationToken cancellationToken = default);
}
