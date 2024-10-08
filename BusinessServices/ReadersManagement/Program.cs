using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.OpenApi.Models;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.HttpProcessors;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Validation.Dto;
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
        o.SourceGeneratorDiscoveredTypes.AddRange(snowcoreBlog.Backend.ReadersManagement.DiscoveredTypes.All);
    })
    .SwaggerDocument()
    .AddCors();

builder.Services.AddSingleton<IValidator<CreateReaderAccountDto>, CreateReaderAccountValidation>();

builder.Services.AddScoped<IReaderRepository, ReaderRepository>();

builder.Services.AddScoped<ValidateCreateReaderAccountStep>();
builder.Services.AddScoped<ValidateReaderAccountNotExistsStep>();
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
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Serializer.Options.SetJsonSerializationContext();
        c.Endpoints.Configurator = ep =>
        {
            ep.PreProcessor<CookieJsonWebTokenProcessor>(Order.Before);
        };
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
    })
    .UseSwaggerGen();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(c =>
    {
        c.EndpointPathPrefix = "/scalar";
        c.DarkMode = true;
    });
}

await app.RunAsync();