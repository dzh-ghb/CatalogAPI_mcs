namespace Catalog.Api.Books.GetBookById;

[Mapper]
public static partial class GetBookByIdMapper
{
	public static partial GetBookByIdResponse ToResponse(
		this GetBookByIdResult result);
}