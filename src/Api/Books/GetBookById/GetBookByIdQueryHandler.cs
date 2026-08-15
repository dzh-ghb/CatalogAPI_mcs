namespace Catalog.Api.Books.GetBookById;

// модель запроса (DTO-модель)
public record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdResult>;

public class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
	public GetBookByIdQueryValidator()
	{
		RuleFor(item => item.Id).NotEmpty().WithMessage("Id не может быть пустым");
	}
}

// результат обработки запроса (DTO-модель)
public record GetBookByIdResult(Book? Book);

public class GetBookByIdQueryHandler
{
	public static async Task<GetBookByIdResult> Handle(
		GetBookByIdQuery query,
		IDocumentSession session,
		CancellationToken cancellationToken)
	{
		var book = await session.LoadAsync<Book>(query.Id, cancellationToken);

		if (book is null)
		{
			throw new BookNotFoundException(query.Id);
		}

		return new GetBookByIdResult(book);
	}
}