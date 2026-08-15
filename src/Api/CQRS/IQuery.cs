// инфраструктура для запросов
namespace Catalog.Api.CQRS;

public interface IQuery<TResponse>
		where TResponse : notnull
{
}