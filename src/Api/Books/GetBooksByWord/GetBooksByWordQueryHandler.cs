namespace Catalog.Api.Books.GetBooksByWord;

// модель запроса (DTO-модель)
public record GetBooksByWordQuery(int? PageNumber = 1, int? PageSize = 5, string Term = "") : IQuery<GetBooksByWordResult>;

public class GetBooksByWordQueryValidator : AbstractValidator<GetBooksByWordQuery>
{
	public GetBooksByWordQueryValidator()
	{
		RuleFor(item => item.Term)
			.NotEmpty()
			.MinimumLength(2);
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
		var books = await session.Query<Book>()
			.Where(item => item.WebStyleSearch(query.Term, "russian"))  // полнотекстовый поиск в веб-стиле - «слово -исключение»
			.ToPagedListAsync(
				query.PageNumber ?? 1,
				query.PageSize ?? 5,
				cancellationToken);

		return new GetBooksByWordResult(books);
	}
}