namespace Catalog.Api.Middleware;

// middleware по конвенции Wolverine
public class RequestTimingMiddleware
{
	private readonly Stopwatch timer = new();

	// вызов до обработки сообщения
	public void Before() => timer.Start();

	// вызов в любом случае после обработки
	public void Finally(ILogger logger, Envelope envelope)
	{
		timer.Stop();

		string info = $" >> Запрос '{envelope.Message?.GetType().Name}' выполняется {timer.Elapsed.TotalMilliseconds} мс";
		if (timer.Elapsed.Seconds >= 2)
		{
			logger.LogWarning(info);
		}
		else
		{
			logger.LogInformation(info);
		}
	}
}