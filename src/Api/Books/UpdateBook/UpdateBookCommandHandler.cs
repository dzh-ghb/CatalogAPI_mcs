namespace Catalog.Api.Books.UpdateBook;

public record UpdateBookCommand(
		Guid Id,
		string Title,
		string Name,
		string Description,
		string ImageUrl,
		decimal Price,
		List<string> Category
) : ICommand<UpdateBookResult>;

//TODO: validator
public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
	public UpdateBookCommandValidator()
	{
		RuleFor(item => item.Id).NotEmpty().WithMessage("Id не может быть пустым"); // верная валидация для Guid?
		RuleFor(item => item.Title).NotEmpty().WithMessage("Title не может быть пустым");
		RuleFor(item => item.Name).NotEmpty().WithMessage("Name не может быть пустым");
		RuleFor(item => item.Price).GreaterThan(0).WithMessage("Price должен быть больше 0");
		RuleFor(item => item.Category).NotEmpty().WithMessage("Category не может быть пустым");
	}
}

public record UpdateBookResult(bool IsSuccess);

public class UpdateBookCommandHandler
{
	public static async Task<UpdateBookResult> Handle(
		UpdateBookCommand command,
		IDocumentSession session,
		CancellationToken cancellationToken)
	{
		var book = await session.LoadAsync<Book>(command.Id, cancellationToken);

		if (book is null)
		{
			throw new BookNotFoundException(command.Id);
		}

		book.UpdateDetails(
			command.Title,
			command.Name,
			command.Description,
			command.ImageUrl,
			command.Category
		);
		book.ChangePrice(command.Price);

		session.Update(book);

		return new UpdateBookResult(true);
	}
}