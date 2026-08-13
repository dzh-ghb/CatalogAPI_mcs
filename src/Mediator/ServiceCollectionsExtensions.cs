namespace Catalog.Mediator;

// метод расширения для регистрации библиотеки, сканирования сборок для поиска обработчиков и подключения сквозных поведений
public static class ServiceCollectionsExtensions
{
	public static IServiceCollection AddMediator(
		this IServiceCollection services,
		Action<MediatorConfigurations> configure
	)
	{
		var configuration = new MediatorConfigurations();
		configure(configuration);
		services.AddScoped<ISender, Sender>();

		foreach (var assembly in configuration.Assemblies)
		{
			var handlers =
				from type in assembly.GetTypes()
				where type is { IsAbstract: false, IsInterface: false }
				from iface in type.GetInterfaces()
				where iface.IsGenericType
					&& iface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
				select (Interface: iface, Implementation: type);

			foreach (var handler in handlers)
			{
				services.AddScoped(handler.Interface, handler.Implementation);
			}
		}

		foreach (var behavior in configuration.OpenBehaviors)
		{
			services.AddScoped(typeof(IPipelineBehavior<,>), behavior);
		}

		return services;
	}
}