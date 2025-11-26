using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.AspireYarpGateway.Extensions;
using snowcoreBlog.Backend.AspireYarpGateway.Features;
using snowcoreBlog.Backend.AspireYarpGateway.Options;
using snowcoreBlog.Backend.AspireYarpGateway.Validation;
using snowcoreBlog.Backend.Gateway.Middleware.Extensions;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Utilities.Dictionary;
using snowcoreBlog.ServiceDefaults.Extensions;
using Yarp.ReverseProxy.Configuration;

namespace Aspire.Hosting;

public static class YarpResourceExtensions
{
    /// <summary>
    /// Adds a YARP resource to the application.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <returns>The builder.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResourceBuilder<YarpResource> AddYarp(this IDistributedApplicationBuilder builder, string name)
    {
        var yarp = builder.Resources.OfType<YarpResource>().FirstOrDefault();
        if (yarp is not default(YarpResource))
        {
            // You only need one yarp resource per application
            throw new InvalidOperationException("A yarp resource has already been added to this application");
        }

        var yarpBuilder = builder.AddResource(new YarpResource(name))
                                 .WithInitialState(new()
                                 {
                                     ResourceType = "Yarp",
                                     Properties = [],
                                 })
                                 .ExcludeFromManifest();

        // Use modern OnInitializeResource pattern for wait support
        yarpBuilder.OnInitializeResource(StartYarpResourceAsync);

        return yarpBuilder;
    }

    private static async Task StartYarpResourceAsync(YarpResource yarpResource, InitializeResourceEvent initEvent, CancellationToken cancellationToken)
    {
        var executionContext = initEvent.Services.GetRequiredService<DistributedApplicationExecutionContext>();
        if (executionContext.IsPublishMode)
        {
            return;
        }

        var logger = initEvent.Logger;
        var notificationService = initEvent.Notifications;
        var serviceProvider = initEvent.Services;

        logger.LogInformation("Starting YARP gateway resource '{ResourceName}'", yarpResource.Name);

        // Publish initial state
        await notificationService.PublishUpdateAsync(yarpResource, s => s with
        {
            ResourceType = "Yarp",
            State = "Starting"
        });

        // We don't want to proxy for yarp resources so force endpoints to not proxy
        var bindings = yarpResource.Annotations.OfType<EndpointAnnotation>().ToList();
        foreach (var b in bindings)
        {
            b.IsProxied = false;
        }

        // Get required services
        var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var eventing = serviceProvider.GetRequiredService<IDistributedApplicationEventing>();

        // Publish BeforeResourceStartedEvent - this is CRITICAL for wait support
        await eventing.PublishAsync(new BeforeResourceStartedEvent(yarpResource, serviceProvider), cancellationToken);

        // Build and configure WebApplication
        var app = await BuildYarpWebApplicationAsync(
            yarpResource,
            hostEnvironment,
            executionContext,
            logger,
            cancellationToken);

        // Start the WebApplication
        await app.StartAsync(cancellationToken);

        // Update endpoint allocations
        var endpoints = yarpResource.Annotations.OfType<EndpointAnnotation>().ToList();
        var server = app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.GetRequiredFeature<IServerAddressesFeature>().Addresses;

        UpdateAllocatedEndpoints(endpoints, addresses);

        // Publish running state
        await notificationService.PublishUpdateAsync(yarpResource, s => s with
        {
            State = "Running",
            Urls = endpoints.Select(ep => new UrlSnapshot(ep.Name, ep.AllocatedEndpoint?.UriString ?? "", IsInternal: false)).ToImmutableArray(),
        });

        logger.LogInformation("YARP gateway resource '{ResourceName}' is now running", yarpResource.Name);

        // Handle graceful shutdown
        cancellationToken.Register(async () =>
        {
            logger.LogInformation("Stopping YARP gateway resource '{ResourceName}'", yarpResource.Name);
            await app.StopAsync(CancellationToken.None);
            await app.DisposeAsync();
        });
    }

    private static async Task<WebApplication> BuildYarpWebApplicationAsync(
        YarpResource yarpResource,
        IHostEnvironment hostEnvironment,
        DistributedApplicationExecutionContext executionContext,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
        {
            ContentRootPath = hostEnvironment.ContentRootPath,
            EnvironmentName = hostEnvironment.EnvironmentName,
            WebRootPath = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot")
        });

        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new ResourceLoggerProvider(logger));

        // Convert environment variables into configuration
        if (yarpResource.TryGetEnvironmentVariables(out var envAnnotations))
        {
            var context = new EnvironmentCallbackContext(executionContext, cancellationToken: cancellationToken);

            foreach (var cb in envAnnotations)
            {
                await cb.Callback(context);
            }

            var dict = new Dictionary<string, string?>();
            foreach (var (k, v) in context.EnvironmentVariables)
            {
                var val = v switch
                {
                    string s => s,
                    IValueProvider vp => await vp.GetValueAsync(context.CancellationToken),
                    _ => throw new NotSupportedException()
                };

                if (val is not null)
                {
                    dict[k.Replace("__", ":")] = val;
                }
            }

            builder.Configuration.AddInMemoryCollection(dict);
        }

        builder.Services.AddServiceDiscovery();

        var proxyBuilder = builder.Services.AddReverseProxy();

        if (yarpResource.RouteConfigs.Count > 0)
        {
            proxyBuilder.LoadFromMemory(yarpResource.RouteConfigs.Values.ToList(), yarpResource.ClusterConfigs.Values.ToList());
        }

        if (yarpResource.ConfigurationSectionName is not null)
        {
            proxyBuilder.LoadFromConfig(builder.Configuration.GetSection(yarpResource.ConfigurationSectionName));
        }

        proxyBuilder.AddServiceDiscoveryDestinationResolver();

        yarpResource.TryGetEndpoints(out var endpoints);
        var defaultScheme = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Contains("https://") == true ? "https" : "http";
        var needsHttps = defaultScheme == "https" || endpoints?.Any(ep => ep.UriScheme == "https") == true;

        if (needsHttps)
        {
            builder.WebHost.UseKestrelHttpsConfiguration();
        }

        builder.Services.ConfigureHttpJsonOptions(static options =>
        {
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.SetJsonSerializationContext();
        });

        builder.Services.Configure<CookiePolicyOptions>(static options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
        });

        builder.Services.Configure<ForwardedHeadersOptions>(static options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        builder.Services.Configure<SecuritySigningOptions>(builder.Configuration.GetSection("Security:Signing"));

        builder.Services.AddOpenTelemetry().ConnectBackendServices();

        builder.Services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<GetUserTokenPairWithPayloadConsumer>();
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

        builder.Services.AddGatewayMiddleware();
        builder.Services.AddSingleton<IValidator<GetUserTokenPairWithPayload>, GetUserTokenPairWithPayloadValidator>();

        builder.Services.AddCors(static x => x.AddDefaultPolicy(static p => p
            .WithOrigins("https://localhost:*/")
            .SetIsOriginAllowed(static host => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

        builder.Services.AddMultipleAuthentications(
            builder.Configuration["Security:Signing:User:SigningKey"]!,
            builder.Configuration["Security:Signing:Admin:SigningKey"]!);

        builder.Services.AddAuthorization(options =>
        {
            foreach (var policy in yarpResource.PoliciesConfigs)
            {
                options.AddPolicy(policy.Item1, policy.Item2);
            }
        });

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        app.UseHttpsRedirection()
           .UseCors()
           .UseCookiePolicy(new()
           {
               MinimumSameSitePolicy = SameSiteMode.Strict,
               HttpOnly = HttpOnlyPolicy.Always,
               Secure = CookieSecurePolicy.Always
           })
           .UseGatewayMiddleware()
           .UseForwardedHeaders()
           .UseAuthentication()
           .UseAuthorization();

        // Configure URLs
        if (endpoints is null || !endpoints.Any())
        {
            var url = $"{defaultScheme}://127.0.0.1:0/";
            app.Urls.Add(url);
        }
        else
        {
            foreach (var ep in endpoints)
            {
                var scheme = ep.UriScheme ?? defaultScheme;
                var url = ep.Port switch
                {
                    null => $"{scheme}://127.0.0.1:0/",
                    _ => $"{scheme}://localhost:{ep.Port}"
                };
                app.Urls.Add(url);
            }
        }

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.MapReverseProxy();

        return app;
    }

    private static void UpdateAllocatedEndpoints(List<EndpointAnnotation> endpoints, ICollection<string> addresses)
    {
        foreach (var ep in endpoints)
        {
            foreach (var url in addresses)
            {
                var uri = new Uri(url);
                if (ep.Port == null || ep.Port == uri.Port)
                {
                    var host = uri.Host is "127.0.0.1" or "[::1]" ? "localhost" : uri.Host;
                    ep.AllocatedEndpoint = new(ep, host, uri.Port);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Loads the YARP configuration from the specified configuration section.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="sectionName">The configuration section name to load from.</param>
    /// <returns>The builder.</returns>
    public static IResourceBuilder<YarpResource> LoadFromConfiguration(this IResourceBuilder<YarpResource> builder, string sectionName)
    {
        builder.Resource.ConfigurationSectionName = sectionName;
        return builder;
    }
    
    public static IResourceBuilder<YarpResource> WithAuthPolicies(this IResourceBuilder<YarpResource> builder, params (string, Action<AuthorizationPolicyBuilder>)[] policies)
    {
        builder.Resource.PoliciesConfigs = policies;
        return builder;
    }
}

/// <summary>
/// Represents a YARP resource.
/// </summary>
/// <param name="name">The name of the resource in the application model.</param>
public class YarpResource(string name) : Resource(name), IResourceWithServiceDiscovery, IResourceWithEnvironment, IResourceWithWaitSupport
{
    // YARP configuration
    internal DictionaryWithDefault<string, RouteConfig> RouteConfigs { get; } = new(defaultValue: new(), 0);
    internal DictionaryWithDefault<string, ClusterConfig> ClusterConfigs { get; } = new(defaultValue: new(), 0);
    internal List<EndpointAnnotation> Endpoints { get; } = [];
    internal string? ConfigurationSectionName { get; set; }
    internal (string, Action<AuthorizationPolicyBuilder>)[] PoliciesConfigs { get; set; } = [];
}

// Helper class for proxying logs to the resource logger
file class ResourceLoggerProvider(ILogger logger) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new ResourceLogger(logger);
    }

    public void Dispose()
    {
    }

    private class ResourceLogger(ILogger logger) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}