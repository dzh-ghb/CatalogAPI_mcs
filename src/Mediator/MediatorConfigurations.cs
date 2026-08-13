using System.Reflection;

namespace Catalog.Mediator;

// класс сборок и сквозных подключений
public class MediatorConfigurations
{
	public List<Assembly> Assemblies { get; set; } = [];
	public List<Type> OpenBehaviors { get; set; } = [];

	public MediatorConfigurations RegisterServicesFromAssembly(Assembly assembly)
	{
		Assemblies.Add(assembly);
		return this;
	}

	public MediatorConfigurations AddOpenBehavior(Type openBehaviorType)
	{
		OpenBehaviors.Add(openBehaviorType);
		return this;
	}
}