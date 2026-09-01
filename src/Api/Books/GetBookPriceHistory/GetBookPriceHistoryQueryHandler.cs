namespace Catalog.Api.Books.GetBookPriceHistory;

// AsOf - необязательный time-travel параметр (поиск ДО указанной даты)
public record GetBookPriceHistoryQuery(Guid Id, DateTimeOffset? AsOf = null) : IQuery<GetBookPriceHistoryResult>;

// перечень изменений отдается неизменяемой коллекцией (только чтение)
public record GetBookPriceHistoryResult(Guid BookId, decimal CurrentPrice, IReadOnlyList<PriceChange> Changes);

public class GetBookPriceHistoryQueryHandler
{
	public static async Task<GetBookPriceHistoryResult> Handle(
		GetBookPriceHistoryQuery query,
		IDocumentSession session,
		CancellationToken cancellationToken
	)
	{
		var history = query.AsOf is not null
			? await session.Events.AggregateStreamAsync<BookPriceHistory>( // живая агрегация (машина времени) -
																																		 // прогон через Apply для событий ДО даты
					query.Id, timestamp: query.AsOf.Value.ToUniversalTime(), token: cancellationToken)
			: await session.LoadAsync<BookPriceHistory>(query.Id, cancellationToken); // inline-проекция (снапшот)

		return history is null
			? throw new BookNotFoundException(query.Id)
			: new GetBookPriceHistoryResult(history.Id, history.CurrentPrice, history.Changes);
	}
}
