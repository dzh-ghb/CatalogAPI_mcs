namespace Catalog.Api.Books.GetBooks;

// модель запроса (DTO-модель)
public record GetBooksQuery(int? PageNumber = 1, int? PageSize = 5) : IQuery<GetBooksResult>;

// результат обработки запроса (DTO-модель)
public record GetBooksResult(IEnumerable<Book> Books);

public class GetBooksQueryHandler
{
	public static async Task<GetBooksResult> Handle(
		GetBooksQuery query,
		IDocumentSession session,
		CancellationToken cancellationToken)
	{
		// await Task.Delay(TimeSpan.FromSeconds(5)); // тест мониторинга времени выполнения запросов

		var books = await session.Query<Book>()
				.ToPagedListAsync(
					query.PageNumber ?? 1,
					query.PageSize ?? 5,
					cancellationToken);

		return new GetBooksResult(books);
	}
}