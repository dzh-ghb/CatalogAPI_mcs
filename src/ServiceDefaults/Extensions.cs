using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting; // неявно подключен в любом веб-проекте,
																				// для доступа из любого Program.cs (по шаблону Aspire)

// метод-расширения для использования в API
public static class Extensions
{
	// метод обобщенный для доступности в любом сервисе решения (не только веб-API, но и воркеры/консольные службы)
	public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder // общий интерфейс всех строителей хоста в .NET
	{
		builder.ConfigureOpenTelemetry();
		builder.AddDefaultHealthChecks();
		builder.Services.AddServiceDiscovery();
		builder.Services.ConfigureHttpClientDefaults(http => // настройки для всех HttpClient (исходящие вызовы)
		{
			http.AddStandardResilienceHandler(); // конвейер устойчивости
			http.AddServiceDiscovery(); // разрешение клиенту использовать логические имена сервисов (доступ по http://api)
		});

		return builder; // fluent-стиль, возврат полученного билдера, чтобы вызовы можно было выстраивать в цепочку
	}

	// метод для сбора инфы о логах, метриках, трейсах для телеметрии
	public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		// подключение провайдера OpenTelemetry к штатной системе логирования (1-й сигнал)
		builder.Logging.AddOpenTelemetry(logging =>
		{
			logging.IncludeFormattedMessage = true;
			logging.IncludeScopes = true;
		});

		builder.Services.AddOpenTelemetry()
			.WithMetrics(metrics => // провайдеры метрик
			{
				metrics.AddAspNetCoreInstrumentation() // счетчики и гистограммы входящих HTTP-запросов
					.AddHttpClientInstrumentation() // счетчики и гистограммы исходящих HTTP-запросов
					.AddRuntimeInstrumentation(); // состояние рантайма
			})
			.WithTracing(tracing => // трейсы
			{
				tracing.AddSource(builder.Environment.ApplicationName) // источник самого приложения
					.AddAspNetCoreInstrumentation()
					.AddSource("Npgsql") // подписка по имени на драйвер PostgreSQL (отдельный спан с инфой о запросах)
					.AddSource("Wolverine") // подписка на конвейер сообщений
					.AddSource("Marten") // подписка на операции хранилища
					.AddHttpClientInstrumentation();
			});

		builder.AddOpenTelemetryExporters();

		return builder;
	}

	// набор проверок работоспособности
	public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		builder.Services.AddHealthChecks() // проверки в DI (может включать проверки базы, брокеров, соседний сервисов и тд)
			.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]); // щас только проверка "жив ли сам процесс"

		return builder;
	}

	// экспортер OTLP (для отправки телеметрии)
	private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		// OTEL_EXPORTER_OTLP_ENDPOINT - содержит адрес приемника
		var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

		if (useOtlpExporter) // нет переменной - экспортер не регистрируется (телеметрия собирается, но никуда не уходит)
		{
			builder.Services.AddOpenTelemetry().UseOtlpExporter();
		}

		return builder;
	}

	// проверка liveness (работает ли приложение, нет - перезапуск) и readiness (все ли зависимости в норме, нет - ожидать)
	public static WebApplication MapDefaultsEndpoints(this WebApplication app)
	{
		if (app.Environment.IsDevelopment())
		{
			app.MapHealthChecks("/health"); // готовность принимать трафик (readiness) - выполнение всех зарегистрированных проверок
			app.MapHealthChecks("/alive", new HealthCheckOptions // liveness
			{
				Predicate = r => r.Tags.Contains("live") // жив ли процесс? - отбор только проверок с тегом live
			});
		}

		return app;
	}
}
