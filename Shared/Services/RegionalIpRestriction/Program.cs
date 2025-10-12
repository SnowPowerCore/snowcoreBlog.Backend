using System.Globalization;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using Marten;
using MassTransit;
using JasperFx.CodeGeneration;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.Localization;
using System.Text.Json;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;
using snowcoreBlog.PublicApi.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<JsonOptions>(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.Configure<RouteOptions>(static options =>
{
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});

builder.WebHost.UseKestrelHttpsConfiguration();

builder.Services.AddHttpContextAccessor();
builder.AddNpgsqlDataSource(connectionName: "db-ip-restrictions-entities");
builder.Services.AddMarten(static options =>
{
    options.RegisterDocumentType<IpRestrictionEntity>();
    options.RegisterDocumentType<RegionRestrictionEntity>();
    options.GeneratedCodeMode = TypeLoadMode.Static;
    options.UseSystemTextJsonForSerialization(configure: static o => o.SetJsonSerializationContext());
    options.Policies.AllDocumentsSoftDeleted();
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.ConfigureHttpJsonOptions(static o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(static options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});
 

builder.AddRedisClient(connectionName: "cache");

const int GlobalVersion = 1;

var jsonStringEnumConverter = new JsonStringEnumConverter();

builder.Services.AddAuthorization()
    .AddFastEndpoints(static options =>
    {
        // no source generator types for now
    })
    .SwaggerDocument(options =>
    {
        options.AutoTagPathSegmentIndex = 0;
        options.ShortSchemaNames = true;
        options.MaxEndpointVersion = GlobalVersion;
        options.SerializerSettings = s =>
        {
            s.Converters.Add(jsonStringEnumConverter);
            s.SetJsonSerializationContext();
            s.PropertyNamingPolicy = null;
        };
    });

builder.Services.AddScoped<IIpRestrictionRepository, IpRestrictionRepository>();
builder.Services.AddScoped<snowcoreBlog.Backend.RegionalIpRestriction.Services.IRequestRestrictionService, snowcoreBlog.Backend.RegionalIpRestriction.Services.RequestRestrictionService>();

var app = builder.Build();

const string DefaultCulture = "en";
var supportedCultures = new[]
{
    new CultureInfo(DefaultCulture)
};

app.UseHttpsRedirection()
    .UseCookiePolicy(new()
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    })
    .UseRequestLocalization(options =>
    {
        options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = [ new AcceptLanguageHeaderRequestCultureProvider() ];
    })
    .UseAuthorization()
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
        c.Serializer.Options.Converters.Add(jsonStringEnumConverter);
        c.Serializer.Options.SetJsonSerializationContext();
    });

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi(c =>
    {
        c.Path = "/openapi/{documentName}.json";
    });
}

await app.RunAsync();