using FastEndpoints;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.HttpMiddleware;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

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

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.UsingRabbitMq((context, config) =>
    {
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
    .AddCors();
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = nameof(snowcoreBlog.Backend.ReadersManagement), Version = "v1" });
});

var swaggerSuffix = "/openapi/v1/swagger.json";

builder.Services.AddSingleton<IReaderRepository, ReaderRepository>();

builder.Services.AddSingleton<ValidateCreateReaderAccountStep>();
builder.Services.AddSingleton<CreateUserForReaderAccountStep>();
builder.Services.AddSingleton<CreateNewReaderEntityStep>();
builder.Services.AddSingleton<SendEmailToNewReaderAccountStep>();

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
        c.DarkMode = true;
    });
}

await app.RunAsync();