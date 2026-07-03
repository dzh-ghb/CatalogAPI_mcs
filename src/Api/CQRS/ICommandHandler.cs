// обработчик команд
namespace Api.CQRS;

// для команд, не возвращающих результат
public interface ICommandHandle<in TCommand>
    : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>
{
}

// для команд, возвращающих результат
public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
}