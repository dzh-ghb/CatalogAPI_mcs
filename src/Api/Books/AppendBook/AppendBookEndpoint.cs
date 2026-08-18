namespace Catalog.Api.Books.AppendBook;

// модель запроса
public record AppendBookRequest(
		string Title,
		string Name,
		string Description,
		string ImageUrl,
		decimal Price,
		List<string> Category
);

// модель ответа
public record AppendBookResponse(Guid Id);

public class AppendBookEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapPost("/books", async (
				AppendBookRequest request,
				IMessageBus bus) =>
		{
			var command = request.ToCommand();
			var result = await bus.InvokeAsync<AppendBookResult>(command);
			var response = result.ToResponse();
			return Results.Ok(response);
		})
		.WithTags("Books")
		.WithSummary("Добавление книги в каталог") // описание
		.Produces<AppendBookResponse>() // тип возвращаемого значения
		.ProducesProblem(StatusCodes.Status400BadRequest); // вероятные проблемы
	}
}