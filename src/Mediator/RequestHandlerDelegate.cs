namespace Catalog.Mediator;

// делегат следующего звена в пайплайне
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();