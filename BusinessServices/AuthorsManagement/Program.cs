using Marten;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(static options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
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

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
// app.UseFastEndpoints();

await app.RunAsync();