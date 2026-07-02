using Marten;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddMarten(option =>
{
    option.Connection(connectionString);
});

var app = builder.Build();

app.MapGet("/test", () => "DZHITS_NDBT");

app.Run();
