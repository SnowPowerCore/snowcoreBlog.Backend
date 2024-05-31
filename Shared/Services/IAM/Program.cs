using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.IAM.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Models;
using snowcoreBlog.Backend.IAM.Options;
using snowcoreBlog.ServiceDefaults.Extensions;
using snowcoreBlog.Backend.IAM.Features.User;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using FluentValidation;
using snowcoreBlog.Backend.IAM.Validation;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Services.Password;
using snowcoreBlog.Backend.IAM.Services.Password;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.Configure<Argon2PasswordHasherOptions>(options =>
{
    options.Strength = Argon2HashStrength.Moderate;
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
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddMartenStores<ApplicationUser, IdentityRole>();
builder.Services
    .AddIdentityCore<ApplicationAdmin>()
    .AddRoles<IdentityRole>()
    .AddMartenStores<ApplicationAdmin, IdentityRole>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CreateUserConsumer>();
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IValidator<CreateUser>, CreateUserValidator>();
builder.Services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

await builder.Build().RunAsync();