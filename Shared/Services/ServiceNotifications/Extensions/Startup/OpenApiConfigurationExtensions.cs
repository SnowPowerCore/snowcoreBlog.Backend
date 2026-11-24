using NSwag;
using Scalar.AspNetCore;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class OpenApiConfigurationExtensions
{
    public static WebApplication UseOpenApiConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi(c =>
            {
                c.Path = "/openapi/{documentName}.json";
                c.PostProcess = (doc, req) =>
                {
                    doc.Host = "https://localhost/api/notifications";
                    doc.Schemes = [OpenApiSchema.Https];
                };
            });
            app.MapScalarApiReference(o =>
            {
                o.DarkMode = true;
            });
        }

        return app;
    }
}