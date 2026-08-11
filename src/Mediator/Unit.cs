namespace Catalog.Mediator;

// void-заглушка для дженериков (например, для заглушки TResponse у команд без возвращаемого результата)
public readonly record struct Unit
{
	public static readonly Unit Value = new();
}