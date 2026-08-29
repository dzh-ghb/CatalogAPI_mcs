namespace Catalog.Api.Books.ChangeBookPrice;

public record ChangeBookPriceCommand(Guid Id, decimal NewPrice) : ICommand<ChangeBookPriceResult>;

public class ChangeBookPriceCommandValidator : AbstractValidator<ChangeBookPriceCommand>
{
	public ChangeBookPriceCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage("Id не может быть пустым");

		RuleFor(x => x.NewPrice)
			.GreaterThan(0)
			.WithMessage("Значение NewPrice должно быть больше 0");
	}
}

public record ChangeBookPriceResult(Guid BookId, decimal OldPrice, decimal NewPrice);

public class ChangeBookPriceCommandHandler
{
	public static async Task<ChangeBookPriceResult> Handle(
		ChangeBookPriceCommand command,
		IDocumentSession session,
		CancellationToken cancellationToken
	)
	{
		var book = await session.LoadAsync<Book>(command.Id, cancellationToken)
			?? throw new BookNotFoundException(command.Id);

		var oldPrice = book.Price;
		book.ChangePrice(command.NewPrice);
		session.Update(book);

		// формирование события
		var priceChanged = new BookPriceChanged(
			book.Id,
			oldPrice,
			book.Price,
			DateTimeOffset.UtcNow
		);

		// стрим для записи события
		var stream = await session.Events.FetchStreamStateAsync(book.Id, cancellationToken);

		if (stream is null) // потока нет (запуск первым событием)
		{
			session.Events.StartStream(book.Id, priceChanged);
		}
		else
		{
			session.Events.Append(book.Id, priceChanged);
		}

		return new ChangeBookPriceResult(book.Id, oldPrice, book.Price);
	}
}