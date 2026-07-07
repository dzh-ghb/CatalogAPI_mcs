namespace Api.Books.DeleteBook;

public record DeleteBookRequest(Guid Id);

public record DeleteBookResponse(bool IsSuccess);

public class DeleteBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/books/{id}", async (
            Guid id,
            ISender sender
        ) =>
        {
            var command = new DeleteBookCommand(id);
            var result = await sender.Send(command);
            var response = result.Adapt<DeleteBookResponse>();
            return Results.Ok(response);
        });
    }
}