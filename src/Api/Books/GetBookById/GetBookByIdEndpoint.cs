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
			var result = await bus.InvokeAsync<Result<GetBookByIdResult>>(query);
			// var response = result.ToResponse();
			// return Results.Ok(response);
			return result.Match<IResult>(
				ok => Results.Ok(ok.ToResponse()),
				error => error.TypeError switch
				{
					ErrorType.NotFound => Results.NotFound(error.Message),
					ErrorType.Validation => Results.BadRequest(error.Message),
					ErrorType.Conflict => Results.Conflict(error.Message),
					_ => Results.Problem(error.Message)
				}
			);
		})
		.WithTags("Books")
		.WithSummary("Получение книги по идентификатору")
		.Produces<GetBookByIdResponse>()
		.ProducesProblem(StatusCodes.Status404NotFound);
	}
}