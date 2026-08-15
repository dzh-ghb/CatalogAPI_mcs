namespace Catalog.Api.Books.DeleteBook;

public record DeleteBookCommand(Guid Id) : ICommand<DeleteBookResult>;

public class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
	public DeleteBookCommandValidator()
	{
		RuleFor(item => item.Id).NotEmpty().WithMessage("Id не может быть пустым");
	}
}

public record DeleteBookResult(bool IsSuccess);

public class DeleteBookCommandHandler
{
	public static async Task<DeleteBookResult> Handle(
		DeleteBookCommand command,
		IDocumentSession session,
		CancellationToken cancellationToken)
	{
		var book = await session.LoadAsync<Book>(command.Id, cancellationToken);

		if (book is null)
		{
			return new DeleteBookResult(false);
		}

		session.Delete(command.Id);

		return new DeleteBookResult(true);
	}
}