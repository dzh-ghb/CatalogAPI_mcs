namespace Catalog.Api.Books.GetBookPriceHistory;

public record PriceChange(
	decimal OldPrice,
	decimal NewPrice,
	DateTimeOffset ChangedAt
);

// модель проекции всех событий потока/стрима, строится через Marten (для чтения истории событий)
public class BookPriceHistory
{
	public Guid Id { get; set; }
	public List<PriceChange> Changes { get; set; } = new();
	public decimal CurrentPrice { get; set; }

	// описание изменения проекции событием по конвенции Marten
	public void Apply(BookPriceChanged priceChanged)
	{
		Changes.Add(new PriceChange(
			priceChanged.OldPrice,
			priceChanged.NewPrice,
			priceChanged.ChangedAt
		));

		CurrentPrice = priceChanged.NewPrice;
	}
}
