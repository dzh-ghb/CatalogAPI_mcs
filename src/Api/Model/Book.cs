namespace Catalog.Api.Model;

// TODO: запретить создание через конструктор (new Book {...})?
public class Book
{
	public Guid Id { get; set; }
	public string Title { get; set; } = default!;
	public string Name { get; set; } = default!;
	public string Description { get; set; } = default!;
	public string ImageUrl { get; set; } = default!;
	public decimal Price { get; set; } = default!;
	public List<string> Category { get; set; } = new();

	// фабричный метод (правило создания модели, единственная возможность создания книг)
	public static Book Create(
		string title,
		string name,
		string description,
		string imageUrl,
		decimal price,
		List<string> category)
	{
		// проверка инвариантов - обязательные правила для объекта
		if (string.IsNullOrWhiteSpace(title))
		{
			throw new BookDomainException("У книги должно быть название");
		}
		if (price <= 0)
		{
			throw new BookDomainException("Цена у книги должна быть больше нуля");
		}

		return new Book
		{
			Title = title,
			Name = name,
			Description = description,
			ImageUrl = imageUrl,
			Price = price,
			Category = category
		};
	}

	public void ChangePrice(decimal newPrice)
	{
		if (newPrice <= 0)
		{
			throw new BookDomainException("Цена у книги должна быть больше нуля");
		}

		if (newPrice > Price * 2 || newPrice < Price / 2)
		{
			throw new BookDomainException($"Цену нельзя изменить больше чем в два раза: " +
			$"{Price} >> {newPrice}");
		}

		Price = newPrice;
	}

	public void UpdateDetails(
		string title,
		string name,
		string description,
		string imageUrl,
		List<string> category)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			throw new BookDomainException("У книги должно быть название");
		}

		Title = title;
		Name = name;
		Description = description;
		ImageUrl = imageUrl;
		Category = category;
	}
}
