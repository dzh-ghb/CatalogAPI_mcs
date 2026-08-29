namespace Catalog.Api.Books.ChangeBookPrice;

// модель события
public record BookPriceChanged(
	Guid BookId,
	decimal OldPrice,
	decimal NewPrice,
	DateTimeOffset ChangedAt
);