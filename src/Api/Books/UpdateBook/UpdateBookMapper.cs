namespace Catalog.Api.Books.UpdateBook;

[Mapper]
public static partial class UpdateBookMapper
{
	public static partial UpdateBookCommand ToCommand(
		this UpdateBookRequest request);

	public static partial UpdateBookResponse ToResponse(
		this UpdateBookResult result);

	public static partial void ApplyTo(
		this UpdateBookCommand command, Book book);
}