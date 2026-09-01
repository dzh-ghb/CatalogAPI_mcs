namespace Catalog.Api.Books.GetBookPriceHistory;

[Mapper]
public static partial class GetBookPriceHistoryMapper
{
	public static partial GetBookPriceHistoryResponse ToResponse(this GetBookPriceHistoryResult result);
}