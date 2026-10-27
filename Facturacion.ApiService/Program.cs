using Facturacion.ApiService.Models;
using Facturacion.ApiService.Repositories;
using Facturacion.ApiService.Repositories.Interfaces;
using Facturacion.ApiService.Services;
using Facturacion.ApiService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var mongoConnectionString =
    builder.Configuration["MongoDb:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Falta configurar MongoDb:ConnectionString.");

var mongoDatabaseName =
    builder.Configuration["MongoDb:DatabaseName"]
    ?? throw new InvalidOperationException(
        "Falta configurar MongoDb:DatabaseName.");

builder.Services.AddSingleton<IMongoClient>(
    _ => new MongoClient(mongoConnectionString));

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<FacturaService>();

var app = builder.Build();

app.UseCors("AngularFrontend");

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/mongodb-test", async (IMongoDatabase database) =>
{
    await database.RunCommandAsync<BsonDocument>(
        new BsonDocument("ping", 1));

    return Results.Ok(new
    {
        mensaje = "Conexión a MongoDB exitosa"
    });
});

// Consultar todas las facturas
app.MapGet("/api/facturas", async ([FromServices] IFacturaRepository repository) =>
{
    var facturas = await repository.ObtenerTodasAsync();
    return Results.Ok(facturas);
});

// Consultar facturas por estado
app.MapGet("/api/facturas/estado/{estado}", async (
    string estado,
    [FromServices] IFacturaRepository repository) =>
{
    var facturas = await repository.ObtenerPorEstadoAsync(estado);
    return Results.Ok(facturas);
});

app.MapPost(
    "/api/facturas/procesar-recordatorios",
    async (FacturaService service) =>
    {
        var cantidad = await service.ProcesarRecordatoriosAsync();

        return Results.Ok(new
        {
            mensaje = "Proceso de recordatorios finalizado.",
            facturasProcesadas = cantidad
        });
    });

app.MapDefaultEndpoints();

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
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
