using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;
using snowcoreBlog.Backend.BusinessServices.AuthorsManagement.Steps;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using snowcoreBlog.Backend.AuthorsManagement.Features;
using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.Services;
using StackExchange.Redis;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    options.SerializerOptions.SetJsonSerializationContext();
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
    opts.UseSystemTextJsonForSerialization(configure: static o => o.SetJsonSerializationContext());
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.Services.AddSingleton<IApplicationLaunchService>(static sp =>
    new AuthorsManagementApplicationLaunchService(sp.GetRequiredService<IConnectionMultiplexer>()));
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<CreateAuthorEntityForExistingUserStep>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CheckAuthorExistsConsumer>();
    busConfigurator.AddConsumer<ReturnClaimsIfUserAuthorConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(static o =>
    {
        o.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        o.SerializerOptions.SetJsonSerializationContext();
    });
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(static options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.AddRedisClient(connectionName: "cache");
//builder.Services.AddFastEndpoints();

builder.Services.AddHostedService(static sp =>
    new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
        sp.GetRequiredService<IApplicationLaunchService>()));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapDefaultEndpoints();
// app.UseFastEndpoints();

await app.RunAsync();