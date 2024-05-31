using FastEndpoints;
using FluentValidation;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.OpenApi.Models;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.HttpMiddleware;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement;
using snowcoreBlog.PublicApi;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.Configure<RouteOptions>(options =>
{
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("Marten"))
    .WithMetrics(metrics => metrics.AddMeter("Marten"));
builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("db-iam-entities"));
builder.Services.AddMarten(opts =>
{
    opts.Policies.AllDocumentsSoftDeleted();
})
    .UseNpgsqlDataSource();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
    });
});
builder.Services.AddMultipleAuthentications(
   builder.Configuration["Security:Signing:User:Key"]!,
   builder.Configuration["Security:Signing:Admin:Key"]!);
builder.Services.AddAuthorization()
    .AddFastEndpoints(static o =>
    {
        o.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
    })
    .AddCors();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = nameof(snowcoreBlog.Backend.ReadersManagement), Version = "v1" });
});

var swaggerSuffix = "/swagger/v1/swagger.json";

builder.Services.AddSingleton<IValidator<CreateReaderAccountDto>, CreateReaderAccountValidation>();

builder.Services.AddScoped<IReaderRepository, ReaderRepository>();

builder.Services.AddScoped<CookieJsonWebTokenMiddleware>();

builder.Services.AddScoped<ValidateCreateReaderAccountStep>();
builder.Services.AddScoped<CreateUserForReaderAccountStep>();
builder.Services.AddScoped<CreateNewReaderEntityStep>();
builder.Services.AddScoped<SendEmailToNewReaderAccountStep>();

var app = builder.Build();

app.UseCookiePolicy(new()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
})
    .UseMiddleware<CookieJsonWebTokenMiddleware>()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Serializer.Options.SetJsonSerializationContext();
        c.Errors.ResponseBuilder = static (failures, ctx, statusCode) =>
        {
            var failuresDict = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    keySelector: e => e.Key,
                    elementSelector: e => e.Select(m => $"{e.Key}: {m.ErrorMessage}").ToArray());

            return ErrorResponseUtilities.ApiResponseWithErrors(
                failuresDict.Values.SelectMany(x => x.Select(s => s)).ToList(), statusCode);
        };
    });

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(swaggerSuffix, $"{nameof(snowcoreBlog.Backend.ReadersManagement)} v1");
        c.RoutePrefix = string.Empty;
    });
    app.MapScalarApiReference(c =>
    {
        c.EndpointPathPrefix = "/scalar";
        c.DarkMode = true;
    });
}

await app.RunAsync();