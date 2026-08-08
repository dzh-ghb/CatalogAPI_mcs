namespace Catalog.Api.Exceptions;

public class BookNotFoundException : Exception
{
    public BookNotFoundException(Guid id) : base($"Книга с id '{id}' не существует")
    {
    }
}