using Marten;
using Marten.Services;
using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.AdminsManagement.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten((StoreOptions options) =>
        {
            options.RegisterDocumentType<AdminEntity>();
            options.RegisterDocumentType<AdminInviteEntity>();
            options.RegisterDocumentType<AdminLoginTokenEntity>();

            // Strong inviter reference enforced ON THE DATABASE LEVEL.
            // The FK column is nullable вЂ” null is reserved for the first ever
            // (bootstrap) admin only; invites always carry a non-null inviter.
            options.Schema.For<AdminEntity>()
                .ForeignKey<AdminEntity>(static x => x.InvitedByAdminId, static x =>
                {
                    x.Name = "admins_fk_admin_invited_by_admin_idx";
                })
                .SoftDeleted();
            options.Schema.For<AdminInviteEntity>()
                .ForeignKey<AdminEntity>(static x => x.InvitedByAdminId, static x =>
                {
                    x.Name = "admins_fk_admin_invite_invited_by_admin_idx";
                })
                .Index(static x => x.TokenHash, static x =>
                {
                    x.Name = "admins_uq_admin_invite_token_hash_idx";
                    x.IsUnique = true;
                });

            _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(_serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        return services;
    }
}