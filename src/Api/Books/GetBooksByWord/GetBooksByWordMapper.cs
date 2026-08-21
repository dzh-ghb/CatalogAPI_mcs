namespace Catalog.Api.Books.GetBooksByWord;

[Mapper]
public static partial class GetBooksByWordMapper
{
	public static partial GetBooksByWordQuery ToQuery(
		this GetBooksByWordRequest request);

	public static partial GetBooksByWordResponse ToResponse(
		this GetBooksByWordResult result);
}