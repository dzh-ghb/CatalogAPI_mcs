namespace Catalog.Api.Books.GetBooks;

// модель запроса (для пагинации)
public record GetBooksRequest(int? PageNumber = 1, int? PageSize = 5);

// модель ответа
public record GetBooksResponse(IEnumerable<Book> Books);

public class GetBooksEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/books", async (
				[AsParameters] GetBooksRequest request,
				IMessageBus bus) =>
		{
			var query = request.ToQuery();
			var result = await bus.InvokeAsync<GetBooksResult>(query);
			var response = result.ToResponse();
			return Results.Ok(response);
		})
		.WithTags("Books")
		.WithSummary("Получение списка книг с пагинацией")
		.Produces<GetBooksResponse>();
	}
}