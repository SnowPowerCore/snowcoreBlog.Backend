using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Marten;
using Microsoft.AspNetCore.HttpOverrides;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using FastEndpoints;
using Marten.Services;
using snowcoreBlog.Backend.Infrastructure;

var serializer = new SystemTextJsonSerializer();

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

builder.Services.Configure<ForwardedHeadersOptions>(static options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
builder.AddNpgsqlDataSource(connectionName: "db-snowcore-blog-entities");
builder.Services.AddMarten(options =>
{
    options.Policies.AllDocumentsSoftDeleted();
    serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
    options.Serializer(serializer);
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.AddRedisClient(connectionName: "cache");

builder.Services.AddScoped<IUrlMappingRepository, UrlMappingRepository>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization()
    .AddAntiforgery()
    .AddFastEndpoints();

var app = builder.Build();

app.UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseAntiforgeryFE(additionalContentTypes: [MediaTypeNames.Application.Json])
    .UseFastEndpoints(c =>
    {
        c.Endpoints.ShortNames = true;
        c.Endpoints.RoutePrefix = default;
        c.Versioning.Prefix = "v";
        c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        c.Serializer.Options.SetJsonSerializationContext();
    });

app.MapDefaultEndpoints();

await app.RunAsync();
