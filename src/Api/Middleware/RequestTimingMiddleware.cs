namespace Catalog.Api.Middleware;

public class RequestTimingMiddleware
{
	private readonly Stopwatch timer = new();

	public void Before() => timer.Start();

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