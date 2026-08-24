namespace Catalog.Api.Books.GetBooksByWord;

// модель запроса
public record GetBooksByWordRequest(int? PageNumber = 1, int? PageSize = 5, string Term = "");

// модель ответа
public record GetBooksByWordResponse(IEnumerable<Book> Books);

public class GetBooksByWordEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/books/search", async (
			[AsParameters] GetBooksByWordRequest request,
			IMessageBus bus) =>
		{
			var query = request.ToQuery();
			var result = await bus.InvokeAsync<GetBooksByWordResult>(query);
			var response = result.ToResponse();
			return Results.Ok(response);
		})
		.WithTags("Books")
		.WithSummary("Поиск книг по слову в названии/авторе/описании (с поддержкой пагинации)")
		.Produces<GetBooksByWordResponse>()
		.ProducesValidationProblem();
	}
}