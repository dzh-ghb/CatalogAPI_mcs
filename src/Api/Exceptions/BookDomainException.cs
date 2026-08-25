namespace Catalog.Api.Exceptions;

// для отлова нарушений бизнес-правил (доменное исключение)
public class BookDomainException(string message) : Exception(message)
{
}