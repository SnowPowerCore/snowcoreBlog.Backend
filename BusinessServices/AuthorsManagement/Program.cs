using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;
using snowcoreBlog.Backend.BusinessServices.AuthorsManagement.Features;
using snowcoreBlog.Backend.BusinessServices.AuthorsManagement.Steps;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});

builder.Services.Configure<CookiePolicyOptions>(static options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<ForwardedHeadersOptions>(static options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
builder.AddNpgsqlDataSource(connectionName: "db-snowcore-blog-entities");
builder.Services.AddMarten(static opts =>
{
    opts.Policies.AllDocumentsSoftDeleted();
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<CreateAuthorEntityForExistingUserStep>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CheckAuthorExistsConsumer>();
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

//builder.Services.AddFastEndpoints();
// builder.Services.AddHostedService(sp =>
//         new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
//             sp.GetRequiredService<IApplicationLaunchService>()));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
// app.UseFastEndpoints();

await app.RunAsync();