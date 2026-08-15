namespace Catalog.Api.Books.DeleteBook;

// public record DeleteBookRequest(Guid Id); // нужен для использования данных из тела запроса (body) или query-параметров

public record DeleteBookResponse(bool IsSuccess);

public class DeleteBookEndpoint : ICarterModule
{
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		// id приходит из route-параметра
		app.MapDelete("/books/{id}", async (
				Guid id,
				IMessageBus bus) =>
		{
			var command = new DeleteBookCommand(id);
			var result = await bus.InvokeAsync<DeleteBookResult>(command);
			var response = result.Adapt<DeleteBookResponse>();
			return Results.Ok(response);
		})
		.WithTags("Books")
		.WithSummary("Удаление книги из каталога")
		.Produces<DeleteBookResponse>();
	}
}