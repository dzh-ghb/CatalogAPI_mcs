namespace Microsoft.Extensions.Hosting; // неявно подключен в любом веб-проекте,
																				// для доступа из любого Program.cs (по шаблону Aspire)

// метод-расширения для использования в API
public static class Extensions
{
	// метод обобщенный для доступности в любом сервисе решения (не только веб-API, но и воркеры/консольные службы)
	public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder // общий интерфейс всех строителей хоста в .NET
	{
		return builder; // fluent-стиль, возврат полученного билдера, чтобы вызовы можно было выстраивать в цепочку
	}
}
