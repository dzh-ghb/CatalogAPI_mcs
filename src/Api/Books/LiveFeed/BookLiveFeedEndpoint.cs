namespace Catalog.Api.Books.LiveFeed;

// поток для клиентов (живая лента, ответ с заголовком Content-Type: text/event-stream); поток сам не закрывается
public class BookLiveFeedEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/books/live", (IBookLiveFeed liveFeed, CancellationToken cancellationToken) =>
		{
			// каждый элемент потока событий мапится в SSE-чанки + ответ формируется через фреймворк
			return TypedResults.ServerSentEvents(liveFeed.Subscribe(cancellationToken), eventType: "price-changed");
		})
		.WithTags("Books")
		.WithSummary("Живая лента изменения цен");
	}
}