using System.Text.Json.Serialization;
using System.Net.Mime;
using System.Text.Json;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Features;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Services;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using StackExchange.Redis;
using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using MinimalStepifiedSystem.Extensions;
using snowcoreBlog.Backend.AuthorsManagement.Steps;

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
builder.Services.AddMarten(opts =>
{
    opts.Policies.AllDocumentsSoftDeleted();
    opts.UseSystemTextJsonForSerialization(configure: static o => o.SetJsonSerializationContext());

    opts.RegisterCompiledQueryType(typeof(AuthorGetByUserIdQuery));
    opts.RegisterCompiledQueryType(typeof(AuthorExistsByUserIdQuery));
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.Services.AddSingleton<IApplicationLaunchService>(static sp =>
    new AuthorsManagementApplicationLaunchService(sp.GetRequiredService<IConnectionMultiplexer>()));
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<CreateAuthorEntityForExistingUserStep>();
builder.Services.AddMassTransit(busConfigurator =>
{
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

const int GlobalVersion = 1;

builder.Services.AddAuthentication();
builder.Services.AddAuthorization()
    .AddAntiforgery()
    .AddFastEndpoints();

builder.Services.AddScoped<UserCookieJsonWebTokenMiddleware>();

builder.Services.AddHostedService(static sp =>
    new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
        sp.GetRequiredService<IApplicationLaunchService>()));

var app = builder.Build();

app.UseStepifiedSystem();
app.UseHttpsRedirection()
    .UseCookiePolicy(new()
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    })
    .UseMiddleware<UserCookieJsonWebTokenMiddleware>()
    .UseAuthentication()
    .UseAuthorization()
    .UseAntiforgeryFE(additionalContentTypes: [MediaTypeNames.Application.Json])
    .UseFastEndpointsDiagnosticsMiddleware()
    .UseFastEndpoints(c =>
    {
        c.Endpoints.NameGenerator = static ctx =>
        {
            var currentName = ctx.EndpointType.Name;
            return currentName.TrimEnd("Endpoint");
        };
        c.Endpoints.ShortNames = true;
        c.Endpoints.RoutePrefix = default;
        c.Versioning.Prefix = "v";
        c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        c.Serializer.Options.SetJsonSerializationContext();
    });

app.MapDefaultEndpoints();

await app.RunAsync();