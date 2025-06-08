using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using MassTransit;
using Marten;
using FluentValidation;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add FastEndpoints
builder.Services.AddFastEndpoints();

// Add MassTransit (RabbitMQ)
builder.Services.AddMassTransit(busConfigurator =>
{
    // Example: busConfigurator.AddConsumer<YourConsumer>();
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("rabbitmq") ?? "amqp://guest:guest@localhost:5672");
        config.ConfigureEndpoints(context);
    });
});

// Add Marten (PostgreSQL)
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("postgres") ?? "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=ims");
    // Example: options.Schema.For<YourEntity>();
});

// Add FluentValidation (register validators manually or via assembly scan if available)
// Example: builder.Services.AddScoped<IValidator<YourType>, YourTypeValidator>();

// Add OpenTelemetry (basic setup)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource("MassTransit");
        tracing.AddSource("Marten");
    })
    .WithMetrics(metrics =>
    {
        metrics.AddMeter("Marten");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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
