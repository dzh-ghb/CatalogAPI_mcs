namespace Catalog.Api.Books.GetBookPriceHistory;

public record GetBookPriceHistoryResponse(Guid BookId, decimal CurrentPrice, IReadOnlyList<PriceChange> Changes);

public class GetBookPriceHistoryEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/books/{id:guid}/price-history", async (
			Guid id,
			DateTimeOffset? asOf,
			IMessageBus bus) =>
		{
			var result = await bus.InvokeAsync<GetBookPriceHistoryResult>(new GetBookPriceHistoryQuery(id, asOf));

			return Results.Ok(result.ToResponse());
		})
		.WithTags("Books")
		.WithSummary("История изменений стоимости книги")
		.Produces<GetBookPriceHistoryResponse>()
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}
