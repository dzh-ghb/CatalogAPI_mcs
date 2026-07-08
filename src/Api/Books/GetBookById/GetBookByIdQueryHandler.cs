namespace Api.Books.GetBookById;

// модель запроса (DTO-модель)
public record GetBookByIdQuery(Guid Id) : IQuery<GetBookByIdResult>;

// результат обработки запроса (DTO-модель)
public record GetBookByIdResult(Book? Book);

public class GetBookByIdQueryHandler(IDocumentSession session) : IQueryHandler<GetBookByIdQuery, GetBookByIdResult>
{
    public async Task<GetBookByIdResult> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await session.LoadAsync<Book>(query.Id, cancellationToken);

        return new GetBookByIdResult(book);
    }
}