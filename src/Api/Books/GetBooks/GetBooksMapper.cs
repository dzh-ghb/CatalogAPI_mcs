namespace Catalog.Api.Books.GetBooks;

[Mapper]
public static partial class GetBooksMapper
{
	public static partial GetBooksQuery ToQuery(
		this GetBooksRequest request);

	public static partial GetBooksResponse ToResponse(
		this GetBooksResult result);
}