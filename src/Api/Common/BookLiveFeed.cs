namespace Catalog.Api.Common;

// контракт шины (паттерн publisher/subscriber)
public interface IBookLiveFeed
{
	// для хранения фактов свершившихся действий (из хендлера)
	void Publish(BookPriceChanged priceChanged);

	// точка входа для клиентов (асинхронная последовательность, поток событий для чтения клиентом)
	IAsyncEnumerable<BookPriceChanged> Subscribe(CancellationToken cancellationToken);
}

// шина/посредник между издателем и подписчиками
public class BookLiveFeed : IBookLiveFeed
{
	// потокобезопасная коллекция для хранения всех подписчиков (одновременное чтение и запись)
	private readonly ConcurrentDictionary<Guid, Channel<BookPriceChanged>> subscribers = new();

	// публикация событий в каналы текущих (активных) подписчиков (без историй/очередей)
	public void Publish(BookPriceChanged priceChanged)
	{
		foreach (var channel in subscribers.Values)
		{
			channel.Writer.TryWrite(priceChanged);
		}
	}

	// вызывается один раз при запуске клиента, создает новый канал и кладет в словарь subscribers
	public async IAsyncEnumerable<BookPriceChanged> Subscribe(
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		// ключ подписки из словаря subscribers
		var subscriberId = Guid.NewGuid();
		var channel = Channel.CreateUnbounded<BookPriceChanged>();
		subscribers[subscriberId] = channel;

		try
		{
			// оповещение будущих клиентов ("бесконечное" чтение событий из канала)
			await foreach (var priceChanged in channel.Reader.ReadAllAsync(cancellationToken))
			{
				yield return priceChanged;
			}
		}
		finally // удаление канала после отключения клиента
		{
			subscribers.TryRemove(subscriberId, out _);
		}
	}
}