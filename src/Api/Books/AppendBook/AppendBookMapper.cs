namespace Catalog.Api.Books.AppendBook;

[Mapper]
public static partial class AppendBookMapper // partial - частичный класс
{
	public static partial AppendBookCommand ToCommand(
		this AppendBookRequest request);

	public static partial AppendBookResponse ToResponse(
		this AppendBookResult result);
}