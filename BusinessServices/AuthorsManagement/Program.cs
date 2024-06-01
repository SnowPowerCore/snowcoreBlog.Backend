using Marten;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Marten"))
    .WithMetrics(metrics => metrics.AddMeter("Marten"));
builder.Services.AddNpgsqlDataSource("db-snowcore-blog-entities");
builder.Services.AddMarten(opts =>
{
    opts.Policies.AllDocumentsSoftDeleted();
})
    .UseNpgsqlDataSource();
//builder.Services.AddFastEndpoints();
// builder.Services.AddHostedService(sp =>
//         new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
//             sp.GetRequiredService<IApplicationLaunchService>()));

var app = builder.Build();

// app.UseFastEndpoints();

await app.RunAsync();