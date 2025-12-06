using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using snowcoreBlog.Backend.RegionalIpRestriction.Middleware;

namespace snowcoreBlog.Backend.Gateway.Middleware.Extensions;

public static class GatewayExtensions
{
    public static IServiceCollection AddGatewayMiddleware(this IServiceCollection serviceCollection) =>
        serviceCollection
            .AddScoped<UserCookieJsonWebTokenMiddleware>();
            // .AddScoped<RequestRestrictionMiddleware>();

    public static IApplicationBuilder UseGatewayMiddleware(this IApplicationBuilder appBuilder) =>
        appBuilder
            .UseMiddleware<UserCookieJsonWebTokenMiddleware>();
            // .UseMiddleware<RequestRestrictionMiddleware>();
}