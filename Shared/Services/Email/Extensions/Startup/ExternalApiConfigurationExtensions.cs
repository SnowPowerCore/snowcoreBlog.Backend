using Apizr;
using Apizr.Extending.Configuring.Common;
using Microsoft.Extensions.Http.Resilience;
using snowcoreBlog.Backend.Email.Api;

namespace snowcoreBlog.Backend.Email.Extensions.Startup;

public static class ExternalApiConfigurationExtensions
{
    public static IServiceCollection AddExternalApiConfiguration(this IServiceCollection services)
    {
        static void OptionsBuilder(IApizrExtendedCommonOptionsBuilder options)
        {
            options.ConfigureHttpClientBuilder(builder => builder
                .AddStandardResilienceHandler(config =>
                {
                    config.Retry = new HttpRetryStrategyOptions
                    {
                        UseJitter = true,
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(0.5)
                    };
                }))
                .WithPriority();
        }

        services.AddApizr(
            registry => registry
                .AddManagerFor<IEmailDisposableApi>(opts => opts.WithBaseAddress("https://disposable.github.io"))
                .AddManagerFor<IStaticEmailDisposableApi>(opts => opts.WithBaseAddress("https://rawcdn.githack.com")),
            OptionsBuilder);

        return services;
    }
}