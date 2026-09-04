using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

// симулятор нагрузки - фоновые запросы на смену цены
var baseUrl = "http://localhost:1905";
const int totalRequests = 40000;
const int durationSeconds = 300;
var interval = TimeSpan.FromSeconds((double)durationSeconds / totalRequests);

using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

var prices = await LoadBooksAsync(client);
if (prices.Count == 0)
{
	Console.Error.WriteLine("Книг не найдено — нечего менять.");
	return 1;
}

var bookIds = prices.Keys.ToList();
var random = new Random();

Console.WriteLine(
	$"Найдено книг: {bookIds.Count}. Делаю {totalRequests} запросов " +
	$"за ~{durationSeconds} секунд (интервал {interval.TotalSeconds:0.00}s). Ctrl+C — остановить."
);

for (var i = 1; i <= totalRequests; i++)
{
	var bookId = bookIds[random.Next(bookIds.Count)];
	var current = prices[bookId];

	var factor = 1 + (random.NextDouble() * 0.4 - 0.2);
	var newPrice = Math.Max(Math.Round(current * (decimal)factor, 2), 1);

	var status = await ChangePriceAsync(client, bookId, newPrice);
	if (status == HttpStatusCode.OK)
	{
		prices[bookId] = newPrice;
		Console.WriteLine($"[{i}/{totalRequests}] {bookId}: {current} -> {newPrice} (200)");
	}
	else
	{
		Console.WriteLine($"[{i}/{totalRequests}] {bookId}: {current} -> {newPrice} (HTTP {(int)status}, пропущено)");
	}

	await Task.Delay(interval);
}

Console.WriteLine("Готово.");
return 0;

static async Task<Dictionary<Guid, decimal>> LoadBooksAsync(HttpClient client)
{
	var response = await client.GetAsync("/books?PageSize=10");
	response.EnsureSuccessStatusCode();

	var json = await response.Content.ReadFromJsonAsync<JsonNode>();
	var books = json!["books"]!.AsArray();

	var prices = new Dictionary<Guid, decimal>();
	foreach (var book in books)
	{
		var id = Guid.Parse(book!["id"]!.GetValue<string>());
		var price = book["price"]!.GetValue<decimal>();
		prices[id] = price;
	}

	return prices;
}

static async Task<HttpStatusCode> ChangePriceAsync(
	HttpClient client,
	Guid bookId,
	decimal newPrice)
{
	var response = await client.PatchAsJsonAsync(
		$"/books/{bookId}/price",
		new { newPrice }
	);
	return response.StatusCode;
}