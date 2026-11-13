using Scalar.AspNetCore;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class OpenApiConfigurationExtensions
{
    public static IServiceCollection AddOpenApiConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();

        return services;
    }

    public static WebApplication UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.MapOpenApi("/openapi/{documentName}.json");
            app.MapScalarApiReference(o =>
            {
                o.DarkMode = true;
            });
        }

        return app;
    }
}