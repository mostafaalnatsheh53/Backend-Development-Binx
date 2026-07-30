using Microsoft.OpenApi;
using MiddlewareDemo.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<GreetingService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddOpenApi();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Middleware 1
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 1 - Before");

    await next();

    Console.WriteLine("Middleware 1 - After");
});

// Middleware 2
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 2 - Before");

    await next();

    Console.WriteLine("Middleware 2 - After");
});

// Middleware 3
app.Use(async (context, next) =>
{
    Console.WriteLine("Middleware 3 - Before");

    await next();

    Console.WriteLine("Middleware 3 - After");
});
//1. Write a small custom middleware that logs each request's method and path to the console, and register it in Program.cs.

app.Use(async (context, next) =>
{
    Console.WriteLine($"Method: {context.Request.Method}");
    Console.WriteLine($"Path: {context.Request.Path}");

    await next();
});

// Enable Controllers
app.MapControllers();

// Existing Weather Endpoint
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}