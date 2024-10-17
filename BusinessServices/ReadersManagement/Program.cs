using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using JasperFx.CodeGeneration;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing.Constraints;
using Scalar.AspNetCore;
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

builder.Services.Configure<MassTransitHostOptions>(static options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.Configure<RouteOptions>(static options =>
{
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});

builder.Services.Configure<JsonOptions>(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.AddServiceDefaults();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry()
    .WithTracing(static tracing => tracing.AddSource("Marten"))
    .WithMetrics(static metrics => metrics.AddMeter("Marten"));
builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("db-iam-entities"));
builder.Services.AddMarten(static opts =>
{
    opts.GeneratedCodeMode = TypeLoadMode.Static;
    opts.Policies.AllDocumentsSoftDeleted();
})
    .UseNpgsqlDataSource();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.ConfigureHttpJsonOptions(o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
    });
});
builder.Services.AddMultipleAuthentications(
   builder.Configuration["Security:Signing:User:Key"]!,
   builder.Configuration["Security:Signing:Admin:Key"]!);
builder.Services
    .AddAuthorization()
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
builder.Services.AddScoped<GenerateTokenForNewReaderAccountStep>();
builder.Services.AddScoped<ReturnCreatedReaderEntityStep>();

var app = builder.Build();

app.UseCookiePolicy(new()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
})
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(static c =>
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
    });

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi(static o => o.Path = "openapi/{documentName}.json");
    app.MapScalarApiReference(static o =>
    {
        o.DarkMode = true;
    });
}

await app.RunAsync();