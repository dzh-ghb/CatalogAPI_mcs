var builder = DistributedApplication.CreateBuilder(args);
// регистрация ресурсов распределённого приложения;
// топология - поднятие и запуск инфраструктуры (контейнеров/БД/сервисов и тд) через Aspire
var postgres = builder.AddPostgres("postgres"); // ресурс-контейнер (образ/порт/пароль автоматом);
																								// особенность: каждый запуск через AppHost - новая БД (у postgres нет тома),
																								// если данные нужно сохранять - строка ниже:
																								// .WithDataVolume("catalog-postgres-data");

var catalogDb = postgres.AddDatabase("catalog-db"); // логическа БД внутри контейнера (имя ресурса = имя строки подключения)

builder.AddProject<Projects.Api>("api") // Projects.Api сгенерированный из подключенной ссылки на сервис тип
	.WithReference(catalogDb) // проброс в процесс Api переменной окружения со строкой подключения к контейнеру с БД
	.WaitFor(catalogDb); // Api не запускается, пока база не будет готова принимать соединения

builder.Build().Run();
