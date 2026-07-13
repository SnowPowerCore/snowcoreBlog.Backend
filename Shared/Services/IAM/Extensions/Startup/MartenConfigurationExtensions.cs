using Marten;
using Marten.Services;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.IAM.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
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

            _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(_serializer);
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        return services;
    }
}