namespace Api.Books.GetBookById;

// модель запроса
public record GetBookByIdRequest(Guid Id);

// модель ответа
public record GetBookByIdResponse(Book? Book);

public class GetBookByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/books/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var query = new GetBookByIdQuery(id);
            var result = await sender.Send(query);
            var response = result.Adapt<GetBookByIdResponse>();
            return Results.Ok(response);
        });
    }
}