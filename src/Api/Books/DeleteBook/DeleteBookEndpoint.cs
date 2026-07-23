namespace Api.Books.DeleteBook;

// public record DeleteBookRequest(Guid Id); // нужен для использования данных из тела запроса (body) или query-параметров

public record DeleteBookResponse(bool IsSuccess);

public class DeleteBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // id приходит из route-параметра
        app.MapDelete("/books/{id}", async (
            Guid id,
            ISender sender) =>
        {
            var command = new DeleteBookCommand(id);
            var result = await sender.Send(command);
            var response = result.Adapt<DeleteBookResponse>();
            return Results.Ok(response);
        });
    }
}