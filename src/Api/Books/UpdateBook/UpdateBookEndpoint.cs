namespace Catalog.Api.Books.UpdateBook;

public record UpdateBookRequest(
		Guid Id,
		string Title,
		string Name,
		string Description,
		string ImageUrl,
		decimal Price,
		List<string> Category
);

public record UpdateBookResponse(bool IsSuccess);

public class UpdateBookEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPut("/books", async (
				UpdateBookRequest request,
				IMessageBus bus) =>
		{
			var command = request.ToCommand();
			var result = await bus.InvokeAsync<UpdateBookResult>(command);
			var response = result.ToResponse();
			return Results.Ok(response);
		})
		.WithTags("Books")
		.WithSummary("Обновление данных о книге")
		.Produces<UpdateBookResponse>()
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}