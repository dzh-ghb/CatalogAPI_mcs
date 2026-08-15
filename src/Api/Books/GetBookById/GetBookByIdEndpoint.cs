namespace Catalog.Api.Books.GetBookById;

// модель запроса
// public record GetBookByIdRequest(Guid Id); // нужен для использования данных из тела запроса (body) или query-параметров

// модель ответа
public record GetBookByIdResponse(Book? Book);

public class GetBookByIdEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		// id приходит из route-параметра
		app.MapGet("/books/{id}", async (
				Guid id,
				IMessageBus bus) =>
		{
			var query = new GetBookByIdQuery(id);
			var result = await bus.InvokeAsync<GetBookByIdResult>(query);
			var response = result.Adapt<GetBookByIdResponse>();
			return Results.Ok(response);
		})
		.WithTags("Books")
		.WithSummary("Получение книги по идентификатору")
		.Produces<GetBookByIdResponse>()
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}