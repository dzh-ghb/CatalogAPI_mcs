namespace Catalog.Mediator;

// интерфейс-маркер (связывание типа запроса с типом ответа при компиляции)
public interface IRequest<out TResponse>
{
}
