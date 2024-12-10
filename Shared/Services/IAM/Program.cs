using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.Infrastructure.Extensions;
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
using JasperFx.CodeGeneration;
using Oakton;
using Microsoft.AspNetCore.Http.Json;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Repositories.Marten;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.ApplyOaktonExtensions();

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.Configure<Argon2PasswordHasherOptions>(static options =>
{
    options.Strength = Argon2HashStrength.Moderate;
});

builder.Services.Configure<JsonOptions>(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("db-iam-entities")!);
//builder.Services.AddNpgsqlDataSource("Host=localhost;Port=54523;Username=postgres;Password=xQ6S1zf+)!kTnjFFCtt(Ks");
builder.Services.AddMarten(static opts =>
{
    opts.RegisterDocumentType<ApplicationAdminEntity>();
    opts.RegisterDocumentType<ApplicationUserEntity>();
    opts.RegisterDocumentType<ApplicationTempUserEntity>();
    opts.RegisterCompiledQueryType(typeof(ApplicationTempUserByEmailQuery));
    opts.RegisterCompiledQueryType(typeof(ApplicationTempUserByNickNameQuery));
    opts.GeneratedCodeMode = TypeLoadMode.Static;
    opts.UseSystemTextJsonForSerialization(configure: o => o.SetJsonSerializationContext());
    opts.Schema.For<ApplicationAdminEntity>().SoftDeleted();
    opts.Schema.For<ApplicationUserEntity>().SoftDeleted();
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();
builder.Services
    .AddIdentityCore<ApplicationUserEntity>()
    .AddRoles<IdentityRole>()
    .AddMartenStores<ApplicationUserEntity, IdentityRole>();
builder.Services
    .AddIdentityCore<ApplicationAdminEntity>()
    .AddRoles<IdentityRole>()
    .AddMartenStores<ApplicationAdminEntity, IdentityRole>();
builder.Services.AddScoped<IApplicationTempUserRepository, ApplicationTempUserRepository>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CreateUserConsumer>();
    busConfigurator.AddConsumer<CreateTempUserConsumer>();
    busConfigurator.AddConsumer<ValidateUserExistsConsumer>();
    busConfigurator.AddConsumer<ValidateTempUserExistsConsumer>();
    busConfigurator.AddConsumer<ValidateUserNickNameWasTakenConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IValidator<CreateUser>, CreateUserValidator>();
builder.Services.AddSingleton<IValidator<CreateTempUser>, CreateTempUserValidator>();
builder.Services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
await app.RunOaktonCommands(args);