var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddMarten(option =>
{
    option.Connection(connectionString);
}).UseLightweightSessions().InitializeWith<InitializeBookDatabase>();

var assembly = typeof(Program).Assembly;

// регистрация MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

// регистрация Carter
builder.Services.AddCarter();

var app = builder.Build();

app.MapCarter();

app.Run();
