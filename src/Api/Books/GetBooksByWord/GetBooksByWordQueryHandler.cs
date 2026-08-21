namespace Catalog.Api.Books.GetBooksByWord;

// модель запроса (DTO-модель)
public record GetBooksByWordQuery(int? PageNumber = 1, int? PageSize = 5, string Term = "") : IQuery<GetBooksByWordResult>;

public class GetBooksByWordQueryQueryValidator : AbstractValidator<GetBooksByWordQuery>
{
	public GetBooksByWordQueryQueryValidator()
	{
		RuleFor(item => item.Term)
			.NotEmpty().WithMessage("Term не может быть пустым")
			.Must(item => item!.Length > 2)
			.WithMessage("Длина Term должна быть больше 2 символов");
	}
}

// результат обработки запроса (DTO-модель)
public record GetBooksByWordResult(IEnumerable<Book> Books);

public class GetBooksByWordQueryHandler
{
	public static async Task<GetBooksByWordResult> Handle(
		GetBooksByWordQuery query,
		IDocumentSession session,
		CancellationToken cancellationToken)
	{
		var word = query.Term.ToLower();

		var books = await session.Query<Book>()
			.Where(item => item.Title.Contains(word, StringComparison.CurrentCultureIgnoreCase) ||
				item.Name.Contains(word, StringComparison.CurrentCultureIgnoreCase) ||
				item.Description.Contains(word, StringComparison.CurrentCultureIgnoreCase))
			.ToPagedListAsync(
				query.PageNumber ?? 1,
				query.PageSize ?? 5,
				cancellationToken);

		return !books.IsEmpty() ? new GetBooksByWordResult(books) : new GetBooksByWordResult([]);
	}
}