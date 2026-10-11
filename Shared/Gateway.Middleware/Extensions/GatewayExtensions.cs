using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using snowcoreBlog.Backend.Gateway.Middleware.Middleware;

namespace snowcoreBlog.Backend.Gateway.Middleware.Extensions;

public static class GatewayExtensions
{
    public static IServiceCollection AddGatewayMiddleware(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<UserCookieJsonWebTokenMiddleware>();

    public static IServiceCollection AddApiAccessRestrictionMiddleware(this IServiceCollection serviceCollection, IConfiguration configuration) =>
        serviceCollection.Configure<ApiAccessRestrictionMiddlewareOptions>(configuration.GetSection(ApiAccessRestrictionMiddlewareOptions.SectionName));

    public static IApplicationBuilder UseGatewayMiddleware(this IApplicationBuilder appBuilder) =>
        appBuilder
            .UseMiddleware<UserCookieJsonWebTokenMiddleware>();

    public static IApplicationBuilder UseApiAccessRestrictionMiddleware(this IApplicationBuilder appBuilder) =>
        appBuilder
            .UseMiddleware<ApiAccessRestrictionMiddleware>();
}