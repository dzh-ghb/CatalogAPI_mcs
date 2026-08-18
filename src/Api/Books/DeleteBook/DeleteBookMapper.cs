namespace Catalog.Api.Books.DeleteBook;

[Mapper]
public static partial class DeleteBookMapper
{
	public static partial DeleteBookResponse ToResponse(
		this DeleteBookResult result);
}