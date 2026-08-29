namespace Catalog.Api.Books.ChangeBookPrice;

[Mapper]
public static partial class ChangeBookPriceMapper
{
	public static partial ChangeBookPriceResponse ToResponse(this ChangeBookPriceResult result);
}