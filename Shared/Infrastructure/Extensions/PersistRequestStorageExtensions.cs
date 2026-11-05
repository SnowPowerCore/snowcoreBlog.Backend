using System.Net;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.Infrastructure.Repositories;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence.ReplayAuthentication;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class PersistRequestStorageExtensions
{
    public const string ReplayHttpClientName = "PersistRequestReplayClient";

    /// <summary>
    /// Registers the claim/checkpoint document types (with the unique idempotency-key index) on a
    /// service's Marten store — call inside the service's <c>AddMarten(options => ...)</c>.
    /// </summary>
    public static StoreOptions AddPersistRequestDocuments(this StoreOptions options)
    {
        options.RegisterDocumentType<PersistedRequestEntity>();
        options.RegisterDocumentType<RequestCheckpointEntity>();
        options.Schema.For<PersistedRequestEntity>()
            .Index(x => x.IdempotencyKey, idx => idx.IsUnique = true);
        return options;
    }

    /// <summary>Registers the claim-store repository (fresh-session semantics; never the business session).</summary>
    public static IServiceCollection AddPersistRequestStorage(this IServiceCollection services)
    {
        return services.AddScoped<IRequestPersistenceRepository, RequestPersistenceRepository>();
    }

    /// <summary>
    /// Registers the full persistence machinery: checkpoint coordinator (consumed by the woven
    /// step advices), replay dispatcher and the replay HTTP client. The recovery poll itself is
    /// a TickerQ job (<see cref="PersistRequestRecoveryJob"/>) — wire TickerQ via
    /// <c>AddTickerQConfiguration</c>/<c>UseTickerQConfigurationAsync</c> (ReadersManagement).
    /// </summary>
    public static IServiceCollection AddPersistRequestProcessing(
        this IServiceCollection services,
        Action<PersistRequestOptions>? configure = null)
    {
        if (configure is not default(Action<PersistRequestOptions>))
        {
            services.Configure(configure);
        }
        else
        {
            services.AddOptions<PersistRequestOptions>();
        }

        return services
            .AddPersistRequestStorage()
            .AddSingleton<StepCheckpointCoordinator>()
            .AddSingleton<IStepCheckpointCoordinator>(static sp => sp.GetRequiredService<StepCheckpointCoordinator>())
            .AddSingleton<IReplayAuthenticationProvider, PassthroughReplayAuthenticationProvider>()
            .AddSingleton<ReplayRequestDispatcher>()
            .AddHttpClient(ReplayHttpClientName, static client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            })
            .ConfigurePrimaryHttpMessageHandler(static () => new SocketsHttpHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AllowAutoRedirect = false
            })
            .Services;
    }
}
