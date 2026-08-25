namespace Catalog.Api.Books.AppendBook;

// модель команды
public record AppendBookCommand(
		string Title,
		string Name,
		string Description,
		string ImageUrl,
		decimal Price,
		List<string> Category
) : ICommand<AppendBookResult>;

// правила валидации данных
public class AppendBookCommandValidator : AbstractValidator<AppendBookCommand>
{
	public AppendBookCommandValidator()
	{
		RuleFor(item => item.Title).NotEmpty().WithMessage("Title не может быть пустым");
		RuleFor(item => item.Name).NotEmpty().WithMessage("Name не может быть пустым");
		RuleFor(item => item.Price).GreaterThan(0).WithMessage("Price должен быть больше 0");
		RuleFor(item => item.Category).NotEmpty().WithMessage("Category не может быть пустым");
	}
}

// модель ответа
public record AppendBookResult(Guid Id);

public class AppendBookCommandHandler/*(IDocumentSession session) // не нужно - связь Wolverine + Marten*/
{
	public static AppendBookResult Handle(
		AppendBookCommand command, // тип обрабатываемого сообщения обязательно первый параметр
		IDocumentSession session)
	{
		var book = Book.Create(
			command.Title,
			command.Name,
			command.Description,
			command.ImageUrl,
			command.Price,
			command.Category);

		session.Store(book);
		// await session.SaveChangesAsync(cancellationToken); // не нужно - настроен транзакционного middleware

		return new AppendBookResult(book.Id);
	}
}