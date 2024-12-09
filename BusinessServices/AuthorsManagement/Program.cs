using Marten;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenTelemetry()
    .WithTracing(static tracing => tracing.AddSource("Marten"))
    .WithMetrics(static metrics => metrics.AddMeter("Marten"));
builder.Services.AddNpgsqlDataSource("db-snowcore-blog-entities");
builder.Services.AddMarten(static opts =>
{
    opts.Policies.AllDocumentsSoftDeleted();
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();
//builder.Services.AddFastEndpoints();
// builder.Services.AddHostedService(sp =>
//         new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
//             sp.GetRequiredService<IApplicationLaunchService>()));

var app = builder.Build();

app.MapDefaultEndpoints();
// app.UseFastEndpoints();

await app.RunAsync();