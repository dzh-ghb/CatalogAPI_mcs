namespace Catalog.Api.Books.LiveFeed;

public class BookLiveFeedEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/books/live", (IBookLiveFeed liveFeed, CancellationToken cancellationToken) =>
		{
			return TypedResults.ServerSentEvents(liveFeed.Subscribe(cancellationToken), eventType: "price-changed");
		})
		.WithTags("Books")
		.WithSummary("Живая лента изменения цен");
	}
}