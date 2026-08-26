namespace Catalog.Api.Model;

// TODO: запретить создание через конструктор (new Book {...})?
public class Book
{
	public Guid Id { get; private set; }
	public string Title { get; private set; } = default!;
	public string Name { get; private set; } = default!;
	public string Description { get; private set; } = default!;
	public string ImageUrl { get; private set; } = default!;
	public decimal Price { get; private set; } = default!;
	public List<string> Category { get; private set; } = new();

	// приватный конструктор - запрет создания экземпляров, кроме как через фабрику Book.Create
	private Book() { }

	// фабричный/доменный метод (правило создания модели, единственная возможность создания книг)
	public static Book Create(
		string title,
		string name,
		string description,
		string imageUrl,
		decimal price,
		List<string> category,
		Guid? id = null) // id - необязательный параметр
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
			Id = id ?? Guid.Empty,
			Title = title,
			Name = name,
			Description = description,
			ImageUrl = imageUrl,
			Price = price,
			Category = category
		};
	}

	// собственное поведение модели
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
