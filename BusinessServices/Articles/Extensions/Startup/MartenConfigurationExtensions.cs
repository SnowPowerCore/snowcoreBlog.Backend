using Ixnas.AltchaNet;
using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Articles.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Article;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using System.Security.Cryptography;

namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer Serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddSingleton(static sp =>
        {
            var key = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return Altcha.CreateServiceBuilder()
                .UseSha256(key)
                .SetExpiryInSeconds(30)
                .UseStore(() =>
                {
                    using var scope = sp.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IAltchaCancellableChallengeStore>();
                })
                .Build();
        });

        services.AddMarten(options =>
        {
            options.RegisterDocumentType<ArticleEntity>();
            options.RegisterDocumentType<ArticleSnapshotEntity>();
            Serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(Serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<UserCookieJsonWebTokenMiddleware>();
        services.AddScoped<IArticleRepository, ArticleRepository>();

        return services;
    }
}