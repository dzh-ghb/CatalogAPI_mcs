namespace Catalog.Api.Books.ChangeBookPrice;

public record ChangeBookPriceRequest(decimal NewPrice);

public record ChangeBookPriceResponse(Guid BookId, decimal OldPrice, decimal NewPrice);

public class ChangeBookPriceEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPatch("/books/{id:guid}/price", async (
				Guid id,
				ChangeBookPriceRequest request,
				IMessageBus bus) =>
		{
			var result = await bus.InvokeAsync<ChangeBookPriceResult>(
				new ChangeBookPriceCommand(id, request.NewPrice));

			return Results.Ok(result.ToResponse());
		})
		.WithTags("Books")
		.WithSummary("Изменение стоимости книги (с записью события в историю)")
		.Produces<ChangeBookPriceResponse>()
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}
