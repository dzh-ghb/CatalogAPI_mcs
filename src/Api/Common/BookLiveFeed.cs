namespace Catalog.Api.Common;

// контракт (механизм publisher subscriber)
public interface IBookLiveFeed
{
	// для хранения фактов свершившихся действий (из хендлера)
	void Publish(BookPriceChanged priceChanged);

	// точка входа для клиентов (для чтения данных)
	IAsyncEnumerable<BookPriceChanged> Subscribe(CancellationToken cancellationToken);
}

public class BookLiveFeed : IBookLiveFeed
{
	// потокобезопасная коллекция для хранения всех подписчиков (одновременное чтение и запись)
	private readonly ConcurrentDictionary<Guid, Channel<BookPriceChanged>> subscribers = new();

	// публикация только для текущих подписчиков (без историй/очередей)
	public void Publish(BookPriceChanged priceChanged)
	{
		foreach (var channel in subscribers.Values)
		{
			channel.Writer.TryWrite(priceChanged);
		}
	}

	public async IAsyncEnumerable<BookPriceChanged> Subscribe(
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		// ключ подписки из словаря subscribers
		var subscriberId = Guid.NewGuid();
		var channel = Channel.CreateUnbounded<BookPriceChanged>();
		subscribers[subscriberId] = channel;

		try
		{
			// оповещение будущих клиентов
			await foreach (var priceChanged in channel.Reader.ReadAllAsync(cancellationToken))
			{
				yield return priceChanged;
			}
		}
		finally
		{
			subscribers.TryRemove(subscriberId, out _);
		}
	}
}