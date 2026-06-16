using Fido2NetLib;
using FluentValidation;
using Marten;
using Marten.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
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
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.ServiceDefaults.Extensions;
using System.Text.Json.Serialization;

[assembly: JasperFx.JasperFxAssembly]

var serializer = new SystemTextJsonSerializer();

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});

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
builder.Services.AddMarten(options =>
{
    options.RegisterDocumentType<ApplicationAdminEntity>();
    options.RegisterDocumentType<ApplicationUserEntity>();
    options.RegisterDocumentType<ApplicationTempUserEntity>();
    options.RegisterDocumentType<Fido2AuthenticatorTransportEntity>();
    options.RegisterDocumentType<Fido2DevicePublicKeyEntity>();
    options.RegisterDocumentType<Fido2PublicKeyCredentialEntity>();
    options.RegisterCompiledQueryType(typeof(ApplicationGetTempUserByEmailQuery));
    options.RegisterCompiledQueryType(typeof(ApplicationTempUserByEmailQuery));
    options.RegisterCompiledQueryType(typeof(ApplicationTempUserByNickNameQuery));
    options.RegisterCompiledQueryType(typeof(PublicKeyCredentialByIdAndCredIdQuery));
    options.RegisterCompiledQueryType(typeof(PublicKeyCredentialGetByUserIdAndCredIdQuery));
    options.RegisterCompiledQueryType(typeof(PublicKeyCredentialsGetByUserIdQuery));
    serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
    options.Serializer(serializer);
    options.Schema.For<ApplicationAdminEntity>().SoftDeleted();
    options.Schema.For<ApplicationUserEntity>().SoftDeleted();
    options.Schema.For<Fido2AuthenticatorTransportEntity>()
        .Index(static x => new { x.PublicKeyId, x.Value }, static x =>
        {
            x.Name = "iam_uq_fido2_auth_trnsprt_cred_id_val_idx";
            x.IsUnique = true;
        })
        .ForeignKey<Fido2PublicKeyCredentialEntity>(static x => x.PublicKeyId!, x => x.Name = "iam_fk_fido2_auth_trnsprt_pub_key_cred_idx")
        .SoftDeletedWithIndex(static x => x.Name = "iam_del_fido2_auth_trnsprt_idx");
    options.Schema.For<Fido2DevicePublicKeyEntity>()
        .Index(static x => new { x.PublicKeyId, x.Value }, static x =>
        {
            x.Name = "iam_uq_fido2_dev_pub_key_cred_id_val_idx";
            x.IsUnique = true;
        })
        .ForeignKey<Fido2PublicKeyCredentialEntity>(static x => x.PublicKeyId!, x => x.Name = "iam_fk_fido2_dev_pub_key_pub_key_cred_idx")
        .SoftDeletedWithIndex(static x => x.Name = "iam_del_fido2_dev_pub_key_idx");
    options.Schema.For<Fido2PublicKeyCredentialEntity>()
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
await app.RunAsync();