var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // библиотека умолчаний Aspire (телеметрия, health-чеки, необходимые маршруты);
															// инструментация должна подниматься раньше "объектов" наблюдения

// для подключения Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// для подключения Scalar
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("catalog-db")!;
builder.Services.AddMarten(option =>
{
	option.Connection(connectionString);
	option.UseNewtonsoftForSerialization( // разрешение использования приватных конструкторов и сеттеров при десериализации
		nonPublicMembersStorage: Weasel.Core.NonPublicMembersStorage.All
	);
	option.Schema.For<Book>().FullTextIndex("russian"); // полнотекстовый индекс для Book с языковым словарем

	option.Projections.Snapshot<BookPriceHistory>(SnapshotLifecycle.Inline); // регистрация проекции
																																					 // (snapshot обновляется в транзакции записи события)
})
.UseLightweightSessions()
.InitializeWith<InitializeBookDatabase>()
.IntegrateWithWolverine(); // мост для работы транзакционного middleware

var assembly = typeof(Program).Assembly;

// // регистрация MediatR
// builder.Services.AddMediatR(config =>
// {
// 	config.RegisterServicesFromAssembly(assembly);
// 	config.AddOpenBehavior(typeof(TimeoutBehavior<,>));
// 	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
// });

builder.Host.UseWolverine(opts =>
{
	opts.Policies.AutoApplyTransactions(); // транзакционный middleware (для работы обработчиков и автосохранения данных)
	opts.UseFluentValidation(); // middleware валидации
	opts.Policies.AddMiddleware<RequestTimingMiddleware>(); // middleware мониторинга времени выполнения запросов
});

builder.Services.AddValidatorsFromAssembly(assembly);

// регистрация Carter
builder.Services.AddCarter();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// одна шина на все приложение
builder.Services.AddSingleton<IBookLiveFeed, BookLiveFeed>();

// настройка времени ожидания отключения клиентов от сервера Kestrel'ом
builder.Services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(1));

// доступ к API с других источников (origin)
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
		policy
			.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());
});

var app = builder.Build();

app.UseExceptionHandler(opt => { });

app.UseCors();

app.MapCarter();

app.MapDefaultsEndpoints(); // проверки работоспособности

if (app.Environment.IsDevelopment())
{
	// app.UseSwagger();
	// app.UseSwaggerUI();
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.Run();
// return await app.RunJasperFxCommands(args); // для демонстрации работы JasperFx