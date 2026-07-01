// инфраструктура для команд
namespace Api.CQRS
{
    // для команд, не возвращающих результат
    public interface ICommand : ICommand<Unit>
    {
    }

    // для команд, возвращающих результат
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}