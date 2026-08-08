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
				ISender sender) =>
		{
			var query = request.Adapt<GetBooksQuery>();
			var result = await sender.Send(query);
			var response = result.Adapt<GetBooksResponse>();
			return Results.Ok(response);
		});
	}
}