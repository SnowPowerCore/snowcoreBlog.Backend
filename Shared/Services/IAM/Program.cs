using System.Text.Json.Serialization;
using Fido2NetLib;
using FluentValidation;
using JasperFx.CodeGeneration;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Oakton;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Features.Admin;
using snowcoreBlog.Backend.IAM.Features.User;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Validation;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.ServiceDefaults.Extensions;
using Weasel.Core;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});
builder.Host.ApplyOaktonExtensions();

builder.Services.Configure<MassTransitHostOptions>(static options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.Configure<ValidStates<HashedStringsVerificationResult>>(static options =>
{
    options.States = [HashedStringsVerificationResult.Success, HashedStringsVerificationResult.SuccessRehashNeeded];
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
builder.AddNpgsqlDataSource(connectionName: "db-iam-entities", configureDataSourceBuilder: b => b.ConnectionStringBuilder.IncludeErrorDetail = true);
//builder.Services.AddNpgsqlDataSource("Host=localhost;Port=54523;Username=postgres;Password=xQ6S1zf+)!kTnjFFCtt(Ks");
builder.Services.AddMarten(static opts =>
{
    opts.RegisterDocumentType<ApplicationAdminEntity>();
    opts.RegisterDocumentType<ApplicationUserEntity>();
    opts.RegisterDocumentType<ApplicationTempUserEntity>();
    opts.RegisterDocumentType<Fido2AuthenticatorTransportEntity>();
    opts.RegisterDocumentType<Fido2DevicePublicKeyEntity>();
    opts.RegisterDocumentType<Fido2PublicKeyCredentialEntity>();
    opts.RegisterCompiledQueryType(typeof(ApplicationGetTempUserByEmailQuery));
    opts.RegisterCompiledQueryType(typeof(ApplicationTempUserByEmailQuery));
    opts.RegisterCompiledQueryType(typeof(ApplicationTempUserByNickNameQuery));
    opts.RegisterCompiledQueryType(typeof(PublicKeyCredentialByIdAndCredIdQuery));
    opts.RegisterCompiledQueryType(typeof(PublicKeyCredentialGetByUserIdAndCredIdQuery));
    opts.RegisterCompiledQueryType(typeof(PublicKeyCredentialsGetByUserIdQuery));
    opts.AutoCreateSchemaObjects = AutoCreate.All;
    opts.GeneratedCodeMode = TypeLoadMode.Static;
    opts.UseSystemTextJsonForSerialization(configure: static o => o.SetJsonSerializationContext());
    opts.Schema.For<ApplicationAdminEntity>().SoftDeleted();
    opts.Schema.For<ApplicationUserEntity>().SoftDeleted();
    opts.Schema.For<Fido2AuthenticatorTransportEntity>()
        .Index(static x => new { x.PublicKeyId, x.Value }, static x =>
        {
            x.Name = "iam_uq_fido2_auth_trnsprt_cred_id_val_idx";
            x.IsUnique = true;
        })
        .ForeignKey<Fido2PublicKeyCredentialEntity>(static x => x.PublicKeyId!, x => x.Name = "iam_fk_fido2_auth_trnsprt_pub_key_cred_idx")
        .SoftDeletedWithIndex(static x => x.Name = "iam_del_fido2_auth_trnsprt_idx");
    opts.Schema.For<Fido2DevicePublicKeyEntity>()
        .Index(static x => new { x.PublicKeyId, x.Value }, static x =>
        {
            x.Name = "iam_uq_fido2_dev_pub_key_cred_id_val_idx";
            x.IsUnique = true;
        })
        .ForeignKey<Fido2PublicKeyCredentialEntity>(static x => x.PublicKeyId!, x => x.Name = "iam_fk_fido2_dev_pub_key_pub_key_cred_idx")
        .SoftDeletedWithIndex(static x => x.Name = "iam_del_fido2_dev_pub_key_idx");
    opts.Schema.For<Fido2PublicKeyCredentialEntity>()
        .Index(static x => x.PublicKeyCredentialId, static x =>
        {
            x.Name = "iam_pk_fido2_pub_key_cred_idx";
            x.IsUnique = true;
        })
        .SoftDeletedWithIndex(static x => x.Name = "iam_del_fido2_pub_key_cred_idx");
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

builder.Services.AddScoped<IHasher, Argon2Hasher>();
builder.Services.AddScoped<IApplicationTempUserRepository, ApplicationTempUserRepository>();
builder.Services.AddScoped<IFido2PublicKeyCredentialRepository, Fido2PublicKeyCredentialRepository>();
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<CheckAndPerformAssertionConsumer>();
    busConfigurator.AddConsumer<ValidateAndCreateUserConsumer>();
    busConfigurator.AddConsumer<CreateTempUserConsumer>();
    busConfigurator.AddConsumer<ValidateUserExistsConsumer>();
    busConfigurator.AddConsumer<ValidateTempUserExistsConsumer>();
    busConfigurator.AddConsumer<ValidateUserNickNameWasTakenConsumer>();
    busConfigurator.AddConsumer<ValidateAndCreateAttestationConsumer>();
    busConfigurator.AddConsumer<ValidateAndCreateAssertionConsumer>();
    busConfigurator.AddConsumer<ValidateAdminExistsConsumer>();
    busConfigurator.AddConsumer<InviteAndCreateAdminConsumer>();
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
builder.Services.AddFido2(builder.Configuration.GetSection(nameof(Fido2)));

builder.Services.AddSingleton<IValidator<LoginUser>, LoginUserValidator>();
builder.Services.AddSingleton<IValidator<CreateUser>, CreateUserValidator>();
builder.Services.AddSingleton<IValidator<CreateTempUser>, CreateTempUserValidator>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
await app.RunOaktonCommands(args);